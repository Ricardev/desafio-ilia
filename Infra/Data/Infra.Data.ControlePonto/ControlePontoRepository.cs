using Domain.ControlePonto;
using Domain.ControlePonto.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.ControlePonto;

public class ControlePontoRepository : IControlePontoRepository
{
    private readonly ControlePontoContext _context;

    public ControlePontoRepository(ControlePontoContext context)
    {
        _context = context;
    }
    
    public Registro RegistrarPonto(Registro registro)
    {
        var registroAdicionado = _context
            .Set<Registro>()
            .Add(registro);
        _context.SaveChanges();
        return registroAdicionado.Entity;
    }

    public IEnumerable<Registro> ObterRelatorio(DateTime mesEAno)
    {
        var registros = _context.Set<Registro>()
            .AsNoTracking()
            .Where(x => x.DiaHora.Month == mesEAno.Month && 
                        x.DiaHora.Year == mesEAno.Year);
        
        return registros.ToList();
    }

    public IEnumerable<Registro> ObterRegistrosDiario(DateTime date)
    {
        var registros = _context.Set<Registro>()
            .AsNoTracking()
            .Where(x => x.DiaHora.Day == date.Day && 
                        x.DiaHora.Month == date.Month && 
                        date.Year == x.DiaHora.Year);
        
        return registros.ToList();
    }
}