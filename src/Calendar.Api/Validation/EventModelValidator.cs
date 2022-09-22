using Calendar.Api.Models;
using FluentValidation;

namespace Calendar.Api.Validation;

/// <summary>
/// Represents a validator that checks a <see cref="EventModel" /> valid.
/// </summary>
public class EventModelValidator : AbstractValidator<EventModel>
{
    /// <summary>
    /// Initializes a <see cref="EventModelValidator" />.
    /// </summary>
    public EventModelValidator()
    {
        RuleFor(e => e.Id).GreaterThan(0);
        Include(new NewEventModelValidator());
    }
}