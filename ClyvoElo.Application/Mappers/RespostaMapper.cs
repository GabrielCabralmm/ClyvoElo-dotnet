using ClyvoElo.Application.DTOs.DispositivosIot;
using ClyvoElo.Application.DTOs.Doutores;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Domain.Entities;

namespace ClyvoElo.Application.Mappers;

public static class RespostaMapper
{
    public static PetResponse ParaResposta(this Pet pet) => new()
    {
        IdPet = pet.IdPet,
        IdTutor = pet.IdTutor,
        NomeTutor = pet.Tutor?.Nome,
        Nome = pet.Nome,
        Especie = pet.Especie,
        Raca = pet.Raca,
        DataNascimento = pet.DataNascimento,
        PesoKg = pet.PesoKg,
    };

    public static EventoClinicoResponse ParaResposta(this EventoClinico evento) => new()
    {
        IdEvento = evento.IdEvento,
        IdPet = evento.IdPet,
        NomePet = evento.Pet?.Nome,
        IdDoutor = evento.IdDoutor,
        NomeDoutor = evento.Doutor?.Nome,
        TipoEvento = evento.TipoEvento,
        Descricao = evento.Descricao,
        DataEvento = evento.DataEvento,
        Status = evento.Status,
    };

    public static DoutorResponse ParaResposta(this Doutor doutor) => new()
    {
        IdDoutor = doutor.IdDoutor,
        Nome = doutor.Nome,
        Crmv = doutor.Crmv,
        Email = doutor.Email,
        IdClinica = doutor.IdClinica,
        NomeClinica = doutor.Clinica?.Nome,
    };

    public static DispositivoIotResponse ParaResposta(this DispositivoIot dispositivo) => new()
    {
        IdDispositivo = dispositivo.IdDispositivo,
        IdPet = dispositivo.IdPet,
        TipoDispositivo = dispositivo.TipoDispositivo,
        DataInstalacao = dispositivo.DataInstalacao,
    };

    public static LeituraSensorResponse ParaResposta(this LeituraSensor leitura) => new()
    {
        IdLeitura = leitura.IdLeitura,
        IdDispositivo = leitura.IdDispositivo,
        TipoMedida = leitura.TipoMedida,
        ValorMedido = leitura.ValorMedido,
        DataHoraLeitura = leitura.DataHoraLeitura,
    };

    public static MensagemResponse ParaResposta(this MensagemAcompanhamento mensagem) => new()
    {
        Id = mensagem.Id,
        IdPet = mensagem.IdPet,
        NomePet = mensagem.NomePet,
        IdTutor = mensagem.IdTutor,
        IdDoutor = mensagem.IdDoutor,
        NomeDoutor = mensagem.NomeDoutor,
        Categoria = mensagem.Categoria,
        Assunto = mensagem.Assunto,
        Conteudo = mensagem.Conteudo,
        DataEnvio = mensagem.DataEnvio,
        Lida = mensagem.Lida,
        DataLeitura = mensagem.DataLeitura,
    };
}
