using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
using MediatR;
using Moq;

namespace UnitTests.Application;

public class ControlePontoApplicationSetups
{
    public ControlePontoApplicationSetups(){}
    public void SetupSendRegistrarPontoCommand(RegistrarPontoCommand command)
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(command,It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Registro(command.DataHora));
    }
}