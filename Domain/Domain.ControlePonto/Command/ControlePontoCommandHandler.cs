using Domain.ControlePonto.Entities;
using Domain.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Domain.ControlePonto.Command;

public class ControlePontoCommandHandler : IRequestHandler<RegistrarPontoCommand, Registro?>
{
    private readonly IControlePontoRepository _controlePontoRepository;
    private readonly IMessageBus _messageBus;

    public ControlePontoCommandHandler(IControlePontoRepository repository, IMessageBus messageBus)
    {
        _controlePontoRepository = repository;
        _messageBus = messageBus;
    }
    
    
    public Task<Registro?> Handle(RegistrarPontoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var hasErrors = ObterErrosAoRegistrarPonto(request.DataHora);
            if (hasErrors)
                return Task.FromResult<Registro?>(null);
            
            var registro = new Registro(request.DataHora);
            var registroAdicionado = _controlePontoRepository.RegistrarPonto(registro);
            return Task.FromResult(registroAdicionado)!;
        }
        catch (Exception)
        {
            return Task.FromResult<Registro?>(null);;
        }

    }

    private bool ObterErrosAoRegistrarPonto(DateTime dataRegistro)
    {
        if (dataRegistro.DayOfWeek == DayOfWeek.Sunday || dataRegistro.DayOfWeek == DayOfWeek.Saturday)
        {
            _messageBus.RaiseValidationError("Sábado e domingo não são permitidos como dia de trabalho",
                StatusCodes.Status403Forbidden);
            return true;
        }
        
        var registrosNoDia = _controlePontoRepository.ObterRegistrosDiario(dataRegistro).ToList();
        
        if (!registrosNoDia.Any())
            return false;
        
        if (registrosNoDia.Count == 4)
        {
            _messageBus.RaiseValidationError("Apenas 4 horários podem ser registrados por dia",
                StatusCodes.Status403Forbidden);
            return true;
        }

        if (registrosNoDia.Count == 2 && dataRegistro.Subtract(registrosNoDia.LastOrDefault()!.DiaHora) < TimeSpan.FromHours(1))
        {
            _messageBus.RaiseValidationError("Deve haver no mínimo 1 hora de almoço",
                StatusCodes.Status403Forbidden);
            return true;
        }
        
        if (registrosNoDia.Any(x => x.DiaHora.Equals(dataRegistro)))
        {
            _messageBus.RaiseValidationError("Horário já registrado",
                StatusCodes.Status409Conflict);
            return true;
        }

        return false;
    }
}