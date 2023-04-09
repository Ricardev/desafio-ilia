using Domain.ControlePonto.Entities;

namespace Domain.ControlePonto;

public interface IControlePontoRepository
{
    public Registro RegistrarPonto(Registro registro);
    public IEnumerable<Registro> ObterRelatorio(DateTime mesEAno);
    public IEnumerable<Registro> ObterRegistrosDiario(DateTime date);
}