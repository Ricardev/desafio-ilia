using Application.ControlePonto.Models;
using AutoMapper;
using Domain.ControlePonto.Command;

namespace Application.ControlePonto.AutoMapper;

public class ControlePontoViewModelToDomain : Profile
{
    public ControlePontoViewModelToDomain()
    {
        CreateMap<MomentoModel, RegistrarPontoCommand>()
            .ConstructUsing(ctor => new RegistrarPontoCommand(DateTime.Parse(ctor.DataHora!)));
    }
}