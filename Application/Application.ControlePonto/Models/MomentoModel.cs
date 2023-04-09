using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Application.ControlePonto.Models;
public class MomentoModel 
{
    public string? DataHora { get; set; }

    public ValidationResult ValidateViewModel()
    {
        MomentoModelValidator validator = new MomentoModelValidator();
        var result = validator.Validate(this);
        return result;
    }
}

public class MomentoModelValidator : AbstractValidator<MomentoModel>
{
    public MomentoModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.DataHora)
            .NotNull()
            .WithMessage("Campo obrigatório não informado")
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString());

        RuleFor(x => x.DataHora)
            .NotEmpty()
            .WithMessage("Campo obrigatório não informado")
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString());
        
        RuleFor(x => x.DataHora)
            .Must(x => Regex.Match(x,
                @"^\d\d\d\d-(0[1-9]|1[012])-([012]\d|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$").Success)
            .WithMessage("Data e hora em formato inválido")
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString());

    }
}