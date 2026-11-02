using System.Text;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetController : ControllerBase
{
    public const string NomeArquivoExportacao = "2TDSPO_2TDSPX_2026_Dataset_Gabriel_Joao.json";

    private readonly IDatasetService _datasetService;

    public DatasetController(IDatasetService datasetService)
    {
        _datasetService = datasetService;
    }

    [HttpGet("pets", Name = "ListarPetsJsonOracle")]
    [AllowAnonymous]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Pets com tutor em JSON montado no Oracle (pkg_clyvo_pet.prc_lista_pets_json)",
        Description = "O JSON é gerado manualmente pela função pkg_clyvo_json.fn_monta_json_pet, sem funções JSON nativas do Oracle. Filtro opcional: especie.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarPets([FromQuery] string? especie, CancellationToken cancellationToken)
    {
        var json = await _datasetService.ListarPetsJsonAsync(especie, cancellationToken);
        return Content(json, "application/json; charset=utf-8");
    }

    [HttpGet("exportar", Name = "ExportarDataset")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(
        Summary = "Exporta o dataset para o MongoDB (pkg_clyvo_pet.prc_exportar_dataset)",
        Description = "Devolve o arquivo JSON com um documento por pet (tutor, eventos clínicos, dispositivos com leituras e alertas), pronto para importar no MongoDB.")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Exportar(CancellationToken cancellationToken)
    {
        var json = await _datasetService.ExportarAsync(cancellationToken);
        var conteudo = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(json);
        return File(conteudo, "application/json", NomeArquivoExportacao);
    }
}
