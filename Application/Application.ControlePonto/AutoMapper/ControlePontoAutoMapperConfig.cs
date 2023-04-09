using AutoMapper;

namespace Application.ControlePonto.AutoMapper;

public static class ControlePontoAutoMapperConfig
{
    public static MapperConfiguration RegisterControlePontoMapping()
    {
        return new MapperConfiguration(configuration =>
        {
            configuration.AddProfile(new ControlePontoDomainToViewModel());
            configuration.AddProfile(new ControlePontoViewModelToDomain());
        });
    }
}