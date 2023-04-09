namespace Domain.ControlePonto.Entities;

public class Registro
{
    public int Id { get; private set; }
    public DateTime DiaHora { get; }

    public Registro(DateTime diaHora)
    {
        DiaHora = diaHora;
    }
}