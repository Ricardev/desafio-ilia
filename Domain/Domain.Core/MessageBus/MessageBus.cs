
using Domain.Core.Interfaces;

namespace Domain.Core.MessageBus;

public class MessageBus : IMessageBus
{
    private ValidationError? ValidationErrors { get; set; }
    
    public ValidationError? GetValidationError()
    {
        ValidationErrors ??= new ValidationError("", 400);
        return ValidationErrors;
    }

    public void RaiseValidationError(string message, int statusCode)
    {
        ValidationErrors ??= new ValidationError(message, statusCode);
    }
}