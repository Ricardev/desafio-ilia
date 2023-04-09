namespace Domain.Core.MessageBus;

public class ValidationError
{
    public string Mensagem { get; }
    public int StatusCode { get; }

    public ValidationError(string message, int statusCode)
    {
        Mensagem = message;
        StatusCode = statusCode;
    }
}