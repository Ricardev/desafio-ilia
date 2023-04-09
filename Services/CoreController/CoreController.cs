using Domain.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreController;

public class CoreController : ControllerBase
{

    protected IMessageBus _messageBus;
    public CoreController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected new IActionResult Response<T>(T? data, int successStatusCode = StatusCodes.Status200OK)
    {
        var validationError = _messageBus.GetValidationError();

        if (string.IsNullOrEmpty(validationError?.Mensagem))
            return new ObjectResult(new ResponseModelSuccess<T?>(data))
            {
                StatusCode = successStatusCode
            };

        return new ObjectResult(new ResponseModelFailure(validationError.Mensagem))
        {
            StatusCode = validationError.StatusCode
        };
    }
}