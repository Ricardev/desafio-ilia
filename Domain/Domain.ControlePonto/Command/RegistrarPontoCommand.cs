using Domain.ControlePonto.Entities;
using MediatR;

namespace Domain.ControlePonto.Command;

public class RegistrarPontoCommand : IRequest<Registro?>
{
    public DateTime DataHora { get; }

    public RegistrarPontoCommand(DateTime dataHora)
    {
        DataHora = dataHora;
    }
}