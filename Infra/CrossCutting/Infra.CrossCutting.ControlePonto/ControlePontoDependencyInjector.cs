using Application.ControlePonto;
using Application.ControlePonto.AutoMapper;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Infra.Data.ControlePonto;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.CrossCutting.ControlePonto;

public static class ControlePontoDependencyInjector
{
    public static IServiceCollection InjectControlePontoDependencies(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddApiVersioning();
        services.AddScoped(x => new ControlePontoContext());
        services.AddScoped<IControlePontoApplication, ControlePontoApplication>();
        services.AddScoped<IControlePontoRepository, ControlePontoRepository>();
        services.AddAutoMapper(typeof(ControlePontoAutoMapperConfig));
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ControlePontoCommandHandler).Assembly));
        return services;
    }
}