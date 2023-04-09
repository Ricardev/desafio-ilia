using Application.ControlePonto.Models;

namespace Application.ControlePonto;

public interface IControlePontoApplication
{
    public Task<RegistroModel?> RegistrarPonto(MomentoModel momento);
    public RelatorioModel? ObterRelatorio(string mes);
}