using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.DispositivosIot;
using ClyvoElo.Application.DTOs.Doutores;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Application.DTOs.Pets;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoElo.Api.Hateoas;

public static class RecursosHateoasExtensions
{
    public static PetResponse ComLinks(this PetResponse pet, IUrlHelper url)
    {
        pet.Links =
        [
            Link(url, "BuscarPetPorId", new { id = pet.IdPet }, "self", "GET"),
            Link(url, "AtualizarPesoPet", new { id = pet.IdPet }, "atualizar_peso", "PUT"),
            Link(url, "RemoverPet", new { id = pet.IdPet }, "remover", "DELETE"),
            Link(url, "ListarEventosDoPet", new { idPet = pet.IdPet }, "eventos_clinicos", "GET"),
            Link(url, "ListarDispositivosDoPet", new { idPet = pet.IdPet }, "dispositivos_iot", "GET"),
            Link(url, "ListarLeiturasDoPet", new { idPet = pet.IdPet }, "leituras_sensor", "GET"),
            Link(url, "ListarMensagens", new { idPet = pet.IdPet }, "mensagens", "GET"),
        ];
        return pet;
    }

    public static EventoClinicoResponse ComLinks(this EventoClinicoResponse evento, IUrlHelper url)
    {
        evento.Links =
        [
            Link(url, "BuscarEventoPorId", new { id = evento.IdEvento }, "self", "GET"),
            Link(url, "AtualizarStatusEvento", new { id = evento.IdEvento }, "atualizar_status", "PUT"),
            Link(url, "RemoverEvento", new { id = evento.IdEvento }, "remover", "DELETE"),
            Link(url, "BuscarPetPorId", new { id = evento.IdPet }, "pet", "GET"),
            Link(url, "BuscarDoutorPorId", new { id = evento.IdDoutor }, "doutor", "GET"),
        ];
        return evento;
    }

    public static DoutorResponse ComLinks(this DoutorResponse doutor, IUrlHelper url)
    {
        doutor.Links =
        [
            Link(url, "BuscarDoutorPorId", new { id = doutor.IdDoutor }, "self", "GET"),
            Link(url, "ListarEventos", new { idDoutor = doutor.IdDoutor }, "eventos_clinicos", "GET"),
        ];
        return doutor;
    }

    public static DispositivoIotResponse ComLinks(this DispositivoIotResponse dispositivo, IUrlHelper url)
    {
        dispositivo.Links =
        [
            Link(url, "BuscarDispositivoPorId", new { id = dispositivo.IdDispositivo }, "self", "GET"),
            Link(url, "ListarLeiturasDoDispositivo", new { idDispositivo = dispositivo.IdDispositivo }, "leituras_sensor", "GET"),
            Link(url, "BuscarPetPorId", new { id = dispositivo.IdPet }, "pet", "GET"),
        ];
        return dispositivo;
    }

    public static LeituraSensorResponse ComLinks(this LeituraSensorResponse leitura, IUrlHelper url)
    {
        leitura.Links =
        [
            Link(url, "BuscarLeituraPorId", new { id = leitura.IdLeitura }, "self", "GET"),
            Link(url, "BuscarDispositivoPorId", new { id = leitura.IdDispositivo }, "dispositivo", "GET"),
        ];
        return leitura;
    }

    public static MensagemResponse ComLinks(this MensagemResponse mensagem, IUrlHelper url)
    {
        mensagem.Links =
        [
            Link(url, "BuscarMensagemPorId", new { id = mensagem.Id }, "self", "GET"),
            Link(url, "MarcarMensagemComoLida", new { id = mensagem.Id }, "marcar_como_lida", "PATCH"),
            Link(url, "RemoverMensagem", new { id = mensagem.Id }, "remover", "DELETE"),
            Link(url, "BuscarPetPorId", new { id = mensagem.IdPet }, "pet", "GET"),
        ];
        return mensagem;
    }

    private static LinkDto Link(IUrlHelper url, string rota, object valores, string rel, string metodo)
    {
        return new LinkDto(url.RouteUrl(rota, valores) ?? string.Empty, rel, metodo);
    }
}
