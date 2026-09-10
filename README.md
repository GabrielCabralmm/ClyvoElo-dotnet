# CLYVO Elo — API (.NET)

API em ASP.NET Core que dá suporte ao CLYVO Elo — a solução construída para o Challenge FIAP 2026 (CLYVO VET), cobrindo a entrega da disciplina de Advanced Business Development with .NET (Sprint 3).

A API consome o mesmo banco Oracle construído na disciplina de Banco de Dados (script `2TDSPO_2TDSPX_2026_CodigoSql_Gabriel_Joao.sql`), com login separado para Tutor e Doutor.

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core + Oracle.EntityFrameworkCore
- Autenticação JWT com dois papéis (Tutor e Doutor), senha com hash bcrypt
- Serilog (logging estruturado, console + arquivo)
- OpenTelemetry (tracing e métricas)
- xUnit + Moq (testes unitários)
- xUnit + WebApplicationFactory (testes de integração)

## Como configurar

1. Preencha a connection string do Oracle em `ClyvoElo.Api/appsettings.json`, na chave `ConnectionStrings:OracleDb`:

```
User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/SEU_SERVICE;
```

2. Rode o script `2TDSPO_2TDSPX_2026_CodigoSql_Gabriel_Joao.sql` inteiro no seu banco (ele já cria as tabelas e carrega a massa de dados, incluindo os logins de demonstração).

## Como rodar

```
cd ClyvoElo.Api
dotnet restore
dotnet run
```

O navegador abre direto em `/swagger`. Todos os endpoints ficam documentados e testáveis ali.

### Login de demonstração

Todos os tutores e doutores da massa de dados usam a senha `clyvo123`. Exemplos:

- Tutor: `gabriel@email.com` / `clyvo123` em `POST /api/Auth/login/tutor`
- Doutor: `ana.ferreira@clyvovet.com` / `clyvo123` em `POST /api/Auth/login/doutor`

O token retornado deve ser colado no botão **Authorize** do Swagger para acessar as rotas protegidas.

### Papéis e permissões

| Ação | Papel exigido |
|---|---|
| Cadastrar pet / atualizar peso | Tutor |
| Registrar evento clínico / atualizar status | Doutor |
| Consultas (GET) | Livre, sem login |

### Valores aceitos pelo banco

As colunas `tipo_evento` e `status` de `evento_clinico` têm restrição `CHECK` no Oracle — só aceitam exatamente estes valores, sempre em maiúsculo:

- `tipoEvento`: `VACINA`, `CHECKUP`, `POS_OPERATORIO`, `MEDICACAO`, `EXAME`
- `status`: `PENDENTE`, `CONCLUIDO`, `ATRASADO`

Qualquer outro valor (ou a mesma palavra com letra minúscula) é recusado pelo banco com erro `ORA-02290`.

## Health Checks

`GET /health` retorna um JSON com o status de cada verificação:

- `api`: confirma que o processo está respondendo
- `oracle_db`: confirma a conexão real com o banco Oracle
- `servico_externo`: confirma conectividade com serviços externos (hoje aponta pra um endpoint público de teste; na prática, é o mesmo padrão que seria usado pra verificar a disponibilidade de um provedor de IA generativa, WhatsApp, etc.)

Para monitorar em produção, esse endpoint pode ser apontado por qualquer ferramenta de uptime (Kubernetes liveness/readiness probe, Azure Monitor, etc.), já que segue o padrão do `Microsoft.Extensions.Diagnostics.HealthChecks`.

## Logs

Os logs saem estruturados, com nível (`INF`/`WRN`/`ERR`) e um `CorrelationId` por requisição — todo log gerado durante uma mesma requisição HTTP carrega o mesmo identificador, o que permite seguir o rastro completo de uma chamada.

- Console: aparece direto ao rodar `dotnet run`
- Arquivo: `ClyvoElo.Api/logs/clyvoelo-AAAAMMDD.log`, um arquivo novo por dia

## Tracing e métricas

Configurado com OpenTelemetry, exportando para o console (`dotnet run` mostra as atividades e métricas coletadas):

- Tracing automático de toda requisição HTTP (ASP.NET Core) e chamadas HTTP de saída
- Um span manual (`RegistrarEventoClinico`) demonstrando instrumentação customizada, com tags do evento registrado
- Métricas de desempenho (tempo de resposta, contagem por status) via instrumentação automática do ASP.NET Core, mais métricas de runtime (uso de memória, GC, threads)

## Rodando os testes

```
dotnet test
```

Isso roda os dois projetos de teste:

- **ClyvoElo.Tests.Unit**: `PetsControllerTests` mocka o `IPetService` com Moq (camada de aplicação/apresentação); `PetServiceTests` usa o EF Core InMemory pra testar as regras do `PetService` isoladas (camada de domínio/serviço) — nenhum dos dois depende do Oracle.
- **ClyvoElo.Tests.Integration**: `AuthTests` e `PetsTests` compartilham a mesma instância da API através de uma Collection Fixture (`ClyvoEloApiCollection`), evitando subir a aplicação mais de uma vez. Sobem a aplicação inteira com `WebApplicationFactory` e testam o fluxo HTTP completo (login, token, rotas protegidas e públicas) contra o banco real — por isso precisam da connection string configurada antes de rodar.

Os testes seguem o padrão de nomenclatura `MetodoTestado_Cenario_ResultadoEsperado`.

## Estrutura do projeto

```
ClyvoElo.Api/
  Controllers/     endpoints da API
  Models/          entidades e DTOs
  Data/            DbContext e mapeamento com o Oracle
  Services/        regras de negócio
  HealthChecks/     verificações de saúde da API
  Telemetry/       fonte de tracing customizada
ClyvoElo.Tests.Unit/
ClyvoElo.Tests.Integration/
```

## Por que existem CriarPetRequest e CriarEventoClinicoRequest

`Pet` e `EventoClinico` têm referências de ida e volta (`Pet` → `Tutor` → lista de `Pets`, por exemplo). Se o `POST` recebesse a entidade inteira, o Swagger tentava montar um exemplo com essas referências se repetindo sem parar, e travava. `CriarPetRequest` e `CriarEventoClinicoRequest` têm só os campos que fazem sentido pra quem está criando o registro — sem essas referências — e o controller monta a entidade completa por dentro antes de salvar.
