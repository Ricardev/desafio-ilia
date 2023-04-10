using System.Linq.Expressions;
using Domain.ControlePonto;
using Domain.ControlePonto.Command;
using Domain.ControlePonto.Entities;
using Moq;
using Moq.AutoMock;

namespace UnitTests.Domain.Command;

[CollectionDefinition(nameof(ControlePontoCommandHandlerCollection))]
public class ControlePontoCommandHandlerCollection : ICollectionFixture<ControlePontoCommandHandlerSetup> {}
public class ControlePontoCommandHandlerSetup
{
    private AutoMocker _mocker;
    public ControlePontoCommandHandler ObterControlePontoCommandHandler()
    {
        _mocker = new AutoMocker();
        return _mocker.CreateInstance<ControlePontoCommandHandler>();
    }

    public void SetupObterRegistrosDiario(List<Registro> listaDeRegistrosDoDia)
    {
        _mocker.GetMock<IControlePontoRepository>()
            .Setup(x => x.ObterRegistrosDiario(It.IsAny<DateTime>()))
            .Returns(listaDeRegistrosDoDia);
    }
    
    public void SetupRegistrarPonto(Registro registro)
    {
        _mocker.GetMock<IControlePontoRepository>()
            .Setup(x => x.RegistrarPonto(It.IsAny<Registro>()))
            .Returns(registro); 
    }

   public void VerifyMethod<T>(Expression<Action<T>> funcaoExecutada,Times quantidadeVezesExecutadas) where T : class
   {
        _mocker.Verify(funcaoExecutada, quantidadeVezesExecutadas);
    }
}