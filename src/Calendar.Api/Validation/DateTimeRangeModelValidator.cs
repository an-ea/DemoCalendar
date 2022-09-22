using Calendar.Api.Models;
using FluentValidation;

namespace Calendar.Api.Validation;

/// <summary>
/// Represents a validator that checks a <see cref="DateTimeRangeModel" /> valid.
/// </summary>
public class DateTimeRangeModelValidator : AbstractValidator<DateTimeRangeModel>
{
    /// <summary>
    /// Initializes a <see cref="DateTimeRangeModelValidator" />.
    /// </summary>
    public DateTimeRangeModelValidator()
    {
        RuleFor(e => e.End).GreaterThan(e => e.Begin)
            .WithMessage(e => $"'{nameof(e.End)}' must be greater than '{nameof(e.Begin)}'");
    }
}