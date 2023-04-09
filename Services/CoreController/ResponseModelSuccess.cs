namespace CoreController;

public class ResponseModelSuccess<T>
{
    public T Data { get; set; }

    public ResponseModelSuccess(T data)
    {
        Data = data;
    }
}