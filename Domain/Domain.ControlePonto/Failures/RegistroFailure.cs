using Domain.ControlePonto.Entities;

namespace Domain.ControlePonto.Failures;

public class RegistroFailure : Registro
{
    public RegistroFailure(DateTime diaHora) : base(diaHora)
    {
    }
}