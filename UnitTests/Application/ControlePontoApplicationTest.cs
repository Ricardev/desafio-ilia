using Application.ControlePonto;
using Application.ControlePonto.Models;
using Domain.ControlePonto.Command;
using MediatR;
using Moq.AutoMock;

namespace UnitTests.Application;

public class ControlePontoApplicationTest
{
    private readonly IControlePontoApplication _controlePontoApplication;
    private readonly AutoMocker _autoMocker;

    public ControlePontoApplicationTest(IControlePontoApplication controlePontoApplication)
    {
        _autoMocker = new AutoMocker();
        _controlePontoApplication = controlePontoApplication;
    }


    [Fact]
    public void RegistrarPontoComSucesso()
    {
        //Arrange
        var momentoModel = new MomentoModel
        {
            DataHora = "2018-08-22T08:00:00"
        };
        var registrarPontoCommand = new RegistrarPontoCommand(DateTime.Parse(momentoModel.DataHora));

        _autoMocker.Setup<IMediator>(x => x.Send(registrarPontoCommand, CancellationToken.None));

        //Act
        _controlePontoApplication.RegistrarPonto(momentoModel);

        //Assert
        
    }
}