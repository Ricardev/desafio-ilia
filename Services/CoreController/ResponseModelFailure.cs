namespace CoreController;

public class ResponseModelFailure
{
    public string Mensagem { get; }

    public ResponseModelFailure(string mensagem)
    {
        Mensagem = mensagem;
    }
}