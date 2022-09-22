using System.ComponentModel.DataAnnotations;

namespace Calendar.Api.Validation;

/// <summary>
/// Represents a validation attribute that checks an id valid. The type of id is <see cref="int" />.
/// </summary>
public class ValidIdAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not int id)
            throw new ArgumentException($"{nameof(value)} must be of type {nameof(Int32)}.");

        
        return id > 0
            ? ValidationResult.Success
            : new ValidationResult($"{validationContext.DisplayName} must be greater than '0'.");
    }
}