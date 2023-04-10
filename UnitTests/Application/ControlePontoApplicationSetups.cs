using Application.ControlePonto;
using Application.ControlePonto.AutoMapper;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
using MediatR;
using Moq;
using Moq.AutoMock;

namespace UnitTests.Application;
[CollectionDefinition(nameof(ControlePontoApplicationCollection))]
public class ControlePontoApplicationCollection : ICollectionFixture<ControlePontoApplicationSetup>{}
public class ControlePontoApplicationSetup
{
    private AutoMocker _mocker;
    public ControlePontoApplication ObterControlePontoApplication()
    {
        _mocker = new AutoMocker();
        _mocker.Use(ControlePontoAutoMapperConfig.RegisterControlePontoMapping().CreateMapper());
        return _mocker.CreateInstance<ControlePontoApplication>();
    }
    public  void SetupSendRegistrarPontoCommand(RegistrarPontoCommand command)
    { 
        _mocker.GetMock<IMediator>()
            .Setup(x => x.Send(It.IsAny<RegistrarPontoCommand>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Registro(command.DataHora));
    }
    public  void SetupObterRelatorioDoMes(List<Registro> registros)
    { 
        _mocker.GetMock<IControlePontoRepository>()
            .Setup(x => x.ObterRelatorio(It.IsAny<DateTime>()))
            .Returns(registros);
    }
}