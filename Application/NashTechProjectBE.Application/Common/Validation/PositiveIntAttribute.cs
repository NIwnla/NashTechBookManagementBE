using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.Validation;

public class PositiveIntAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is int intValue && intValue > 0)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The value must be a positive integer.");
    }
}