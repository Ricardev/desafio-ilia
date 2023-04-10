using System.ComponentModel;
using Application.ControlePonto;
using Application.ControlePonto.Models;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
using Domain.Core.Interfaces;
using Moq;

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
    
    [Fact(DisplayName = "Obter Relatorio do Mes com Sucesso")]
    [Category("Success")]
    public void ObterRelatorioDoMesComSuccesso()
    {
        //Arrange
        var registros = RegistrosDoMes();
        _setups.SetupObterRelatorioDoMes(registros);
        var relatorioDoMes = new RelatorioModel
        {
            Mes = "2023-04",
            HorasTrabalhadas = "PT24H0M0S",
            HorasDevidas = "PT136H0M0S",
            HorasExcedentes = "0S",
        };
        
        //Act
        var result = _controlePontoApplication.ObterRelatorio("2023-04");

        //Assert
        Assert.Equal(relatorioDoMes.Mes, result.Mes);
        Assert.Equal(relatorioDoMes.HorasDevidas, result.HorasDevidas);
        Assert.Equal(relatorioDoMes.HorasExcedentes, result.HorasExcedentes);
        Assert.Equal(relatorioDoMes.HorasTrabalhadas, result.HorasTrabalhadas);
        _setups.VerifyMethod<IControlePontoRepository>(x => x.ObterRelatorio(It.IsAny<DateTime>()), 
            Times.Once());
    }

    [Fact(DisplayName = "Obter Relatorio do Mes com Falha Formatacao Invalida")]
    [Category("Fail")]
    public void ObterRelatorioDoMesComFalhaFormatacaoInvalida()
    {
        //Act
        var result = _controlePontoApplication.ObterRelatorio("2023-04-02");
        
        //Assert
        _setups.VerifyMethod<IMessageBus>( x=> x.
            RaiseValidationError("A formatação da data está errada. Por favor, use AAAA-MM e meses entre 1 e 12.",
                400), Times.Once());
        Assert.Null(result);
    }
    
    [Fact(DisplayName = "Obter Relatorio do Mes com Falha Relatorio Nao Encontrado")]
    [Category("Fail")]
    public void ObterRelatorioDoMesComFalhaRelatorioNaoEncontrado()
    {
        //Arrange
        _setups.SetupObterRelatorioDoMes(new List<Registro>());
        
        //Act
        var result = _controlePontoApplication.ObterRelatorio("2023-04");
        
        //Assert
        _setups.VerifyMethod<IMessageBus>( x=> x.
            RaiseValidationError("Relatório não encontrado",
                404), Times.Once());
        Assert.Null(result);
    }


    private List<Registro> RegistrosDoMes()
    {
        return new List<Registro>
        {
            new(new DateTime(2023, 4, 3, 8, 0, 0)),
            new(new DateTime(2023, 4, 3, 12, 0, 0)),
            new(new DateTime(2023, 4, 3, 13, 0, 0)),
            new(new DateTime(2023, 4, 3, 17, 0, 0)),
            new(new DateTime(2023, 4, 4, 8, 0, 0)),
            new(new DateTime(2023, 4, 4, 12, 0, 0)),
            new(new DateTime(2023, 4, 4, 13, 0, 0)),
            new(new DateTime(2023, 4, 4, 17, 0, 0)),
            new(new DateTime(2023, 4, 5, 8, 0, 0)),
            new(new DateTime(2023, 4, 5, 12, 0, 0)),
            new(new DateTime(2023, 4, 5, 13, 0, 0)),
            new(new DateTime(2023, 4, 5, 17, 0, 0))
        };
    }
}