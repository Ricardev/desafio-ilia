using System.ComponentModel;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
using Domain.Core.Interfaces;
using Moq;

namespace UnitTests.Domain.Command;

[Collection(nameof(ControlePontoCommandHandlerCollection))]
public class ControlePontoCommandHandlerTest
{
    private readonly ControlePontoCommandHandlerSetup _setup;
    private readonly ControlePontoCommandHandler _commandHandler;

    public ControlePontoCommandHandlerTest(ControlePontoCommandHandlerSetup commandHandlerSetup)
    {
        _setup = commandHandlerSetup;
        _commandHandler = _setup.ObterControlePontoCommandHandler();
    }

    [Theory(DisplayName = "Criacao do Registro do Ponto com Sucesso")]
    [Category("Success")]
    [MemberData(nameof(RegistroDoPonto))]
    public async void CriacaoDoRegistroDoPontoComSucesso(RegistrarPontoCommand pontoBatido, List<Registro> registrosDoDia)
    {
        //Arrange
        _setup.SetupObterRegistrosDiario(registrosDoDia);
        _setup.SetupRegistrarPonto(new Registro(pontoBatido.DataHora));
        
        //Act
        var result = await _commandHandler.Handle(pontoBatido, It.IsAny<CancellationToken>());
        
        //Assert
        Assert.Equal(pontoBatido.DataHora, result.DiaHora);
        
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.ObterRegistrosDiario(It.IsAny<DateTime>()),Times.Once());
        
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.RegistrarPonto(It.IsAny<Registro>()),Times.Once());
    }
    
    [Theory(DisplayName = "Criacao do Registro do Ponto com Falha Sábado e Domingo")]
    [Category("Fail")]
    [MemberData(nameof(RegistroPontoSabadoEDomingo))]
    public async void FalhaCriacaoDoRegistroFinalDeSemana(DateTime dataDoPonto)
    {
        //Act
        var result = await _commandHandler.Handle(new RegistrarPontoCommand(dataDoPonto),
            It.IsAny<CancellationToken>());
        
        //Assert
        Assert.Null(result);
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.ObterRegistrosDiario(It.IsAny<DateTime>()),Times.Never());

        _setup.VerifyMethod<IControlePontoRepository>(x =>
            x.RegistrarPonto(It.IsAny<Registro>()), Times.Never());
        
        _setup.VerifyMethod<IMessageBus>(x =>
            x.RaiseValidationError("Sábado e domingo não são permitidos como dia de trabalho", 403),
            Times.Once());
    }

    [Fact(DisplayName = "Criacao do Registro do Ponto com Falha Já Existem 4 Pontos No dia")]
    [Category("Fail")]
    public async void FalhaCriacaoDoRegistroJaExistemQuatroHorariosRegistrados()
    {
        
        //Arrange
        var registrosNoDia = new List<Registro>
        {
            new (new DateTime(2023, 4, 10, 8, 0, 0)),
            new (new DateTime(2023, 4, 10, 12, 0, 0)),
            new (new DateTime(2023, 4, 10, 13, 0, 0)),
            new (new DateTime(2023, 4, 10, 17, 0, 0))
        };
            
        _setup.SetupObterRegistrosDiario(registrosNoDia);
        
        //Act
        var result = await _commandHandler.Handle(new RegistrarPontoCommand(new DateTime(2023, 4, 10, 18, 0, 0)),
            It.IsAny<CancellationToken>());
        
        //Assert
        Assert.Null(result);
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.ObterRegistrosDiario(It.IsAny<DateTime>()),Times.Once());

        _setup.VerifyMethod<IControlePontoRepository>(x =>
            x.RegistrarPonto(It.IsAny<Registro>()), Times.Never());
        
        _setup.VerifyMethod<IMessageBus>(x =>
                x.RaiseValidationError("Apenas 4 horários podem ser registrados por dia", 403),
            Times.Once());
    }
    
    [Fact(DisplayName = "Criacao do Registro do Ponto com Falha Deve Haver Uma Hora de Almoco")]
    [Category("Fail")]
    public async void FalhaCriacaoDoRegistroDeveHaverUmaHoraDeAlmoco()
    {
        
        //Arrange
        var registrosNoDia = new List<Registro>
        {
            new (new DateTime(2023, 4, 10, 8, 0, 0)),
            new (new DateTime(2023, 4, 10, 12, 0, 0))
        };
        var pontoBatido = new RegistrarPontoCommand(new DateTime(2023, 4, 10, 12, 59, 59));
        _setup.SetupObterRegistrosDiario(registrosNoDia);
        
        //Act
        var result = await _commandHandler.Handle(pontoBatido,
            It.IsAny<CancellationToken>());
        
        //Assert
        Assert.Null(result);
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.ObterRegistrosDiario(It.IsAny<DateTime>()),Times.Once());

        _setup.VerifyMethod<IControlePontoRepository>(x =>
            x.RegistrarPonto(It.IsAny<Registro>()), Times.Never());
        
        _setup.VerifyMethod<IMessageBus>(x =>
                x.RaiseValidationError("Deve haver no mínimo 1 hora de almoço", 403),
            Times.Once());
    }
    
    [Fact(DisplayName = "Criacao do Registro do Ponto com Falha Horario Ja Registrado")]
    [Category("Fail")]
    public async void FalhaCriacaoDoRegistroHorarioJaRegistrado()
    {
        
        //Arrange
        var registrosNoDia = new List<Registro>
        {
            new (new DateTime(2023, 4, 10, 8, 0, 0)),
            new (new DateTime(2023, 4, 10, 12, 0, 0)),
            new (new DateTime(2023, 4, 10, 13, 0, 0))
        };
        var pontoBatido = new RegistrarPontoCommand(new DateTime(2023, 4, 10, 13, 0, 0));
        _setup.SetupObterRegistrosDiario(registrosNoDia);
        
        //Act
        var result = await _commandHandler.Handle(pontoBatido,
            It.IsAny<CancellationToken>());
        
        //Assert
        Assert.Null(result);
        _setup.VerifyMethod<IControlePontoRepository>(x => 
            x.ObterRegistrosDiario(It.IsAny<DateTime>()),Times.Once());

        _setup.VerifyMethod<IControlePontoRepository>(x =>
            x.RegistrarPonto(It.IsAny<Registro>()), Times.Never());
        
        _setup.VerifyMethod<IMessageBus>(x =>
                x.RaiseValidationError("Horário já registrado", 409),
            Times.Once());
    }
    
    private static IEnumerable<object[]> RegistroDoPonto()
    {
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,8,0,0)),
            new List<Registro>()
        };
        
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,12,45,12)),
            new List<Registro>
            {
                new (new DateTime(2023,04,04,8,0,0))
            }
        };
        
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,13,45,13)),
            new List<Registro>
            {
                new (new DateTime(2023,04,04,8,0,0)),
                new (new DateTime(2023,04,04,12,45,12))
            }
        };
        
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,17,0,0)),
            new List<Registro>
            {
                new (new DateTime(2023,04,04,8,0,0)),
                new (new DateTime(2023,04,04,12,45,12)),
                new (new DateTime(2023,04,04,13,45,13))
            }
        };
    }

    private static IEnumerable<object[]> RegistroPontoSabadoEDomingo()
    {
        yield return new object[]
        {
            new DateTime(2023, 4, 9, 8, 0, 0)
        };

        yield return new object[]
        {
            new DateTime(2023, 4, 8, 8, 0, 0)
        };
    }
}