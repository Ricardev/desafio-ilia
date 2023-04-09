using Domain.Core.MessageBus;

namespace Domain.Core.Interfaces;

public interface IMessageBus
{
    ValidationError? GetValidationError();
    void RaiseValidationError(string message, int statusCode);
}