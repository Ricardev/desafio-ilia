using System.ComponentModel;
using Application.ControlePonto;
using Application.ControlePonto.Models;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;

namespace UnitTests.Application;

[Collection(nameof(ControlePontoApplicationCollection))]
public class ControlePontoApplicationTest
{
    private readonly IControlePontoApplication _controlePontoApplication;
    private readonly ControlePontoApplicationSetup _setups;

    public ControlePontoApplicationTest(ControlePontoApplicationSetup controleApplicationSetup)
    {
        _setups = controleApplicationSetup;
        _controlePontoApplication = _setups.ObterControlePontoApplication();
    }


    [Fact(DisplayName = "Registrando ponto com sucesso")]
    [Category("Success")]
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
    
    [Theory(DisplayName = "Obter Relatorio do Mes com Sucesso")]
    [Category("Success")]
    [MemberData(nameof(RelatoriosDoMes))]
    public void ObterRelatorioDoMesComSuccesso(List<Registro> registros)
    {
        //Arrange
        _setups.SetupObterRelatorioDoMes(registros);
        
        //Act
        var result = _controlePontoApplication.ObterRelatorio("2023-03");

        //Assert
        
    }

    private IEnumerator<object[]> RelatoriosDoMes()
    {
        yield return new object[]
        {
            new List<Registro>
            {
                new (new DateTime(2023, 4,3, 8, 0,0 )),
                new (new DateTime(2023, 4,3, 12, 0,0 )),
                new (new DateTime(2023, 4,3, 13, 0,0 )),
                new (new DateTime(2023, 4,3, 17, 0,0 )),
                new (new DateTime(2023, 4,4, 8, 0,0 )),
                new (new DateTime(2023, 4,4, 12, 0,0 )),
                new (new DateTime(2023, 4,4, 13, 0,0 )),
                new (new DateTime(2023, 4,4, 17, 0,0 )),
                new (new DateTime(2023, 4,5, 8, 0,0 )),
                new (new DateTime(2023, 4,5, 12, 0,0 )),
                new (new DateTime(2023, 4,5, 13, 0,0 )),
                new (new DateTime(2023, 4,5, 17, 0,0 ))
            }
        };
    }
}