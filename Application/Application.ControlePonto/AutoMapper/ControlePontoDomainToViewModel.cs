using Application.ControlePonto.Models;
using AutoMapper;
using Domain.ControlePonto.Entities;

namespace Application.ControlePonto.AutoMapper;

public class ControlePontoDomainToViewModel : Profile
{
    public ControlePontoDomainToViewModel()
    {
        CreateMap<Registro, RegistroModel>()
            .ConstructUsing(ctor => new RegistroModel()
            {
                Dia = ctor.DiaHora,
                Horarios = new List<string>
                {
                    ctor.DiaHora.ToLongTimeString()
                }
            });
    }
}