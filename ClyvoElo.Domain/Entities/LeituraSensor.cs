using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Domain.Entities;

public class LeituraSensor
{
    public int IdLeitura { get; set; }
    public int IdDispositivo { get; set; }
    public string TipoMedida { get; set; } = string.Empty;
    public decimal ValorMedido { get; set; }
    public DateTime DataHoraLeitura { get; set; }

    public DispositivoIot? Dispositivo { get; set; }

    public static LeituraSensor Criar(int idDispositivo, string tipoMedida, decimal valorMedido, DateTime? dataHoraLeitura)
    {
        if (string.IsNullOrWhiteSpace(tipoMedida))
        {
            throw new RegraDeNegocioException("O tipo de medida é obrigatório.");
        }

        var dataHora = dataHoraLeitura ?? DateTime.Now;
        if (dataHora > DateTime.Now.AddMinutes(5))
        {
            throw new RegraDeNegocioException("A data da leitura não pode estar no futuro.");
        }

        return new LeituraSensor
        {
            IdDispositivo = idDispositivo,
            TipoMedida = tipoMedida.Trim(),
            ValorMedido = valorMedido,
            DataHoraLeitura = dataHora,
        };
    }
}
