namespace Application.ControlePonto.Models;

public class RelatorioDasHorasTrabalhadasModel
{
    
    public int HorasTrabalhadas { get; }
    public int MinutosTrabalhados { get; }
    public int SegundosTrabalhados { get; }
    public int MesDoRelatorio { get; }
    public int AnoDoRelatorio { get; }
    public int QuantidadeDiasUteisNoMes { get; }
    public RelatorioDasHorasTrabalhadasModel(int ano,int mes,int horas, int minutos, int segundos, int quantidadeDiasUteisNoMes)
    {
        AnoDoRelatorio = ano;
        MesDoRelatorio = mes;
        HorasTrabalhadas = horas;
        MinutosTrabalhados = minutos;
        SegundosTrabalhados = segundos;
        QuantidadeDiasUteisNoMes = quantidadeDiasUteisNoMes;
    }

    
}