using Calendar.Api.Models;
using FluentValidation;

namespace Calendar.Api.Validation;

/// <summary>
/// Represents a validator that checks a <see cref="NewEventModel" /> valid.
/// </summary>
public class NewEventModelValidator : AbstractValidator<NewEventModel>
{
    /// <summary>
    /// Initializes a <see cref="NewEventModelValidator" />.
    /// </summary>
    public NewEventModelValidator()
    {
        RuleFor(e => e.Subject).NotEmpty().MaximumLength(100);
        RuleFor(e => e.Description).NotEmpty().MaximumLength(500);
        RuleFor(e => e.Begin).NotEmpty();
        RuleFor(e => e.End).GreaterThan(e => e.Begin)
            .WithMessage(e => $"'{nameof(e.End)}' must be greater than '{nameof(e.Begin)}'");
    }
}