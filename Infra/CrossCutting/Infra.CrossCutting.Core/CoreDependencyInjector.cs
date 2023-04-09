using Domain.Core.Interfaces;
using Domain.Core.MessageBus;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.CrossCutting.Core;

public static class CoreDependencyInjector
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageBus, MessageBus>();
        return services;
    }
}