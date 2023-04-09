using System.ComponentModel;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
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
    public void CriacaoDoRegistroDoPontoComSucesso(RegistrarPontoCommand pontoBatido, List<Registro> registrosDoDia)
    {
        //Arrange
        _setup.SetupObterRegistrosDiario(registrosDoDia);
        //Act
        _commandHandler.Handle(pontoBatido, It.IsAny<CancellationToken>());
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
            new RegistrarPontoCommand(new DateTime(2023,04,04,8,0,0)),
            new List<Registro>
            {
                new (new DateTime(2023,04,04,12,45,12))
            }
        };
        
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,8,0,0)),
            new List<Registro>()
        };
        
        yield return new object[]
        {
            new RegistrarPontoCommand(new DateTime(2023,04,04,8,0,0)),
            new List<Registro>()
        };
    }
}