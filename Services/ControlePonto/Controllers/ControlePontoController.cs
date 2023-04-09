using Application.ControlePonto;
using Application.ControlePonto.Models;
using CoreController;
using Domain.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace ControlePonto.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}")]
public class ControlePontoController : CoreController.CoreController
{
    private readonly IControlePontoApplication _controlePontoApplication;
    
    public ControlePontoController(IControlePontoApplication controlePontoApplication, IMessageBus messageBus): base(messageBus)
    {
        _controlePontoApplication = controlePontoApplication;
    }
    /// <summary>
    /// Bater ponto.
    /// </summary>
    /// <description> Registrar um horário da jornada diária de trabalho </description>
    /// <param name="momento"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("batidas")]
    [ProducesResponseType(typeof(RegistroModel),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseModelFailure),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseModelFailure),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBatidas([FromBody] MomentoModel momento)
    {
        var validationResult = momento.ValidateViewModel();
        if (!validationResult.IsValid)
        {
            int.TryParse(validationResult.Errors?.FirstOrDefault()?.ErrorCode, out var statusCodes);
            _messageBus.RaiseValidationError($"{validationResult.Errors?.FirstOrDefault()?.ErrorMessage}",
                statusCodes);
            return Response<MomentoModel?>(null);
        }
            
        return Response(await _controlePontoApplication.RegistrarPonto(momento), 
            successStatusCode: StatusCodes.Status201Created) ;
    }
    
    /// <summary>
    /// Relatório mensal
    /// </summary>
    /// <description>
    /// Geração de relatório mensal de usuário.
    /// </description>
    /// <param name="mes">A data deve estar no formato AAAA-MM</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RelatorioModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModelFailure), StatusCodes.Status404NotFound)]
    [Route("folhas-de-ponto/{mes}")]
    public IActionResult GetRelatorioMensal(string mes)
    {
        return Response(_controlePontoApplication.ObterRelatorio(mes), 
            successStatusCode:StatusCodes.Status200OK);
    }

}
