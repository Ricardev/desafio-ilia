namespace Application.ControlePonto.Models;

public class RelatorioModel
{
    public string Mes { get; set; }
    public string HorasTrabalhadas { get; set; }
    public string HorasExcedentes { get; set; }
    public string HorasDevidas { get; set; }
    public IEnumerable<RegistroModel> Registros { get; set; }
}