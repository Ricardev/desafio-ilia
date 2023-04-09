using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Application.ControlePonto.Models;
using AutoMapper;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Domain.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.ControlePonto;

public class ControlePontoApplication : IControlePontoApplication
{
    private readonly IMapper _mapper;
    private readonly IMediator _eventHandler;
    private readonly IControlePontoRepository _controlePontoRepository;
    private readonly IMessageBus _messageBus;

    public ControlePontoApplication(IMapper mapper, IMediator eventHandler, 
        IControlePontoRepository controlePontoRepository, IMessageBus messageBus)
    {
        _mapper = mapper;
        _eventHandler = eventHandler;
        _controlePontoRepository = controlePontoRepository;
        _messageBus = messageBus;
    }
    
    public async Task<RegistroModel?> RegistrarPonto(MomentoModel momento)
    {
        var registrarPontoCommand = _mapper.Map<RegistrarPontoCommand>(momento);
        var registro = await _eventHandler.Send(registrarPontoCommand);
        if (registro == null)
            return null;
        var registroModel = _mapper.Map<RegistroModel>(registro);
        return registroModel;
    }

    public RelatorioModel? ObterRelatorio(string mes)
    {
        if (!Regex.Match(mes, @"^\d\d\d\d-(0[1-9]|1[012])").Success)
        {
            _messageBus.RaiseValidationError("A formatação da data está errada. Por favor, use AAAA-MM",
                StatusCodes.Status400BadRequest);
            return null;
        }
        var formatInfo = new DateTimeFormatInfo();
        formatInfo.YearMonthPattern = "yyyy-MM";
        
        var dateFilter = DateTime.Parse(mes, formatInfo);
        var registros = _controlePontoRepository.ObterRelatorio(dateFilter).ToList();
        
        if (!registros.Any())
        {
            _messageBus.RaiseValidationError("Relatório não encontrado", StatusCodes.Status404NotFound);
            return null;
        }
        
        var listaRegistrosUnidosPorDia = registros
            .GroupBy(x => x.DiaHora.Day)
            .Select(x => x.ToList())
            .ToList()
            .Select(x => new RegistroModel
            {
                Dia = x.FirstOrDefault()!.DiaHora, 
                Horarios = x.Select(registro => registro.DiaHora.ToLongTimeString()).ToList()
            }).ToList();

        var relatorioDasHorasTrabalhadas = CriarRelatorioDasHorasTrabalhadas(listaRegistrosUnidosPorDia);
        
        var relatorioModel = new RelatorioModel
        {
            Mes = mes,
            HorasDevidas = CalcularHorasDevidas(relatorioDasHorasTrabalhadas),
            HorasExcedentes = CalcularHorasExcedentes(relatorioDasHorasTrabalhadas),
            HorasTrabalhadas = CalcularHorasTrabalhadas(relatorioDasHorasTrabalhadas),
            Registros = listaRegistrosUnidosPorDia
        };
  
        return relatorioModel;
    }

    private string CalcularHorasDevidas(RelatorioDasHorasTrabalhadasModel relatorioDasHoras)
    {
        var totalDeHorasParaTrabalharNoMes = relatorioDasHoras.QuantidadeDiasUteisNoMes * 8;

        if (totalDeHorasParaTrabalharNoMes - relatorioDasHoras.HorasTrabalhadas < 0)
            return "0S";
        
        decimal horasDevidasEmSegundos = totalDeHorasParaTrabalharNoMes * 3600 -
                                     (relatorioDasHoras.HorasTrabalhadas * 3600 +
                                      relatorioDasHoras.MinutosTrabalhados * 60 + relatorioDasHoras.SegundosTrabalhados);
        
        var horasDevidas = Math.Round(horasDevidasEmSegundos / 3600, MidpointRounding.ToZero);
        
        var minutosDevidos = Math.Round((horasDevidasEmSegundos / 60)%60, MidpointRounding.ToZero);
        
        var segundosDevidos = Math.Round(horasDevidasEmSegundos%60%60, MidpointRounding.ToZero);
        
        var horasMinutosSegundosDevidos = new StringBuilder("PT");
        
        if(horasDevidas> 0)
            horasMinutosSegundosDevidos.Append(horasDevidas + "H");
            
        if(minutosDevidos > 0)      
            horasMinutosSegundosDevidos.Append(minutosDevidos + "M");
            
        horasMinutosSegundosDevidos.Append(segundosDevidos + "S");
        
        return horasMinutosSegundosDevidos.ToString();
    }

    private string CalcularHorasExcedentes(RelatorioDasHorasTrabalhadasModel relatorioDasHoras)
    {
        var totalDeHorasParaTrabalharNoMes = relatorioDasHoras.QuantidadeDiasUteisNoMes * 8;
        var horasExcedentes = relatorioDasHoras.HorasTrabalhadas - totalDeHorasParaTrabalharNoMes;
        if (horasExcedentes < 0)
            return "0S";

        var horasMinutosSegundosExedentes = new StringBuilder("PT")
            .Append(horasExcedentes + "H")
            .Append(relatorioDasHoras.MinutosTrabalhados + "M")
            .Append(relatorioDasHoras.SegundosTrabalhados + "S")
            .ToString();

        return horasMinutosSegundosExedentes;

    }
    
    private string CalcularHorasTrabalhadas(RelatorioDasHorasTrabalhadasModel relatorioDasHoras)
    {
        var horasTrabalhadas = new StringBuilder("PT");
        
        if(relatorioDasHoras.HorasTrabalhadas> 0)
            horasTrabalhadas.Append(relatorioDasHoras.HorasTrabalhadas + "H");
            
        if(relatorioDasHoras.MinutosTrabalhados > 0)      
            horasTrabalhadas.Append(relatorioDasHoras.MinutosTrabalhados + "M");
        
        horasTrabalhadas.Append(relatorioDasHoras.SegundosTrabalhados + "S");
        
        return horasTrabalhadas.ToString();

    }

    private RelatorioDasHorasTrabalhadasModel CriarRelatorioDasHorasTrabalhadas(List<RegistroModel> registros)
    {
        var dateTimeFomartInfo = new DateTimeFormatInfo();
        dateTimeFomartInfo.ShortTimePattern = "HH-MM-SS";
        
        var listaDeHorarios = registros.Select(x => x.Horarios);
        var totalDiasConcluidos = listaDeHorarios
            .Where(x => x.Count == 4).ToList();

        var totalHorasTrabalhadas = totalDiasConcluidos
            .Sum(x => TimeSpan.Parse(x[1], dateTimeFomartInfo)
                .Subtract(TimeSpan.Parse(x[0], dateTimeFomartInfo))
                .Add(TimeSpan.Parse(x[3], dateTimeFomartInfo))
                .Subtract(TimeSpan.Parse(x[2], dateTimeFomartInfo)).Hours);

        var totalMinutosTrabalhados = totalDiasConcluidos
            .Sum(x => TimeSpan.Parse(x[1], dateTimeFomartInfo)
            .Subtract(TimeSpan.Parse(x[0], dateTimeFomartInfo))
            .Add(TimeSpan.Parse(x[3], dateTimeFomartInfo))
            .Subtract(TimeSpan.Parse(x[2], dateTimeFomartInfo)).Minutes);
        
        var totalSegundosTrabalhados = totalDiasConcluidos
            .Sum(x => TimeSpan.Parse(x[1], dateTimeFomartInfo)
            .Subtract(TimeSpan.Parse(x[0], dateTimeFomartInfo))
            .Add(TimeSpan.Parse(x[3], dateTimeFomartInfo))
            .Subtract(TimeSpan.Parse(x[2], dateTimeFomartInfo)).Seconds);

        var registroReferenciaParaMesAno = registros.FirstOrDefault()!.Dia;
        
        return new RelatorioDasHorasTrabalhadasModel(registroReferenciaParaMesAno.Year, 
            registroReferenciaParaMesAno.Month,
            totalHorasTrabalhadas, 
            totalMinutosTrabalhados, 
            totalSegundosTrabalhados,
            QuantidadeDiasUteisNoMes(registroReferenciaParaMesAno.Year,registroReferenciaParaMesAno.Month));
    }

    private int QuantidadeDiasUteisNoMes(int year, int month)
    {
        
        var diasUteisNoMes = Enumerable
            .Range(1,DateTime.DaysInMonth(year, month))
            .Select(x => new DateTime(year, month, x))
            .Count(x => x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday);
        return diasUteisNoMes;
    }
    
}