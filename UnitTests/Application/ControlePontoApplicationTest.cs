using Application.ControlePonto;
using Application.ControlePonto.Models;
using Domain.ControlePonto.Command;
using MediatR;
using Moq;

namespace UnitTests.Application;

public class ControlePontoApplicationTest
{
    private readonly IControlePontoApplication _controlePontoApplication;
    private readonly ControlePontoApplicationSetups _setups;

    public ControlePontoApplicationTest(IControlePontoApplication controlePontoApplication)
    {
        _setups = new ControlePontoApplicationSetups();
        _controlePontoApplication = controlePontoApplication;
    }


    [Fact(DisplayName = "Registrando ponto com sucesso")]
    public async void RegistrarPontoComSucesso()
    {
        //Arrange
        var momentoModel = new MomentoModel
        {
            DataHora = "2018-08-22T08:00:00"
        };
        var data = DateTime.Parse(momentoModel.DataHora);
        var registroModel = new RegistroModel
        {
            Dia = data,
            Horarios = new List<string>
            {
                data.ToLongTimeString()
            }
        };
        var registrarPontoCommand = new RegistrarPontoCommand(DateTime.Parse(momentoModel.DataHora));
        _setups.SetupSendRegistrarPontoCommand(registrarPontoCommand);

        //Act
        var result = await _controlePontoApplication.RegistrarPonto(momentoModel);

        //Assert
        Assert.Equal(registroModel.Dia, result.Dia);
        Assert.Contains(registroModel.Horarios.FirstOrDefault(), result.Horarios);
    }
}