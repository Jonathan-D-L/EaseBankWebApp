using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DataAccessLibrary.Infrastructure.Validation
{
    public class CustomDecimalAttribute : ValidationAttribute
    {
        private const string Pattern = @"^(?!-)(?!0)\d{1,16}(\,\d{1,3})?$";

        public CustomDecimalAttribute()
        {
            ErrorMessage = "The given value is not valid for Amount.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var stringValue = value.ToString();

            if (stringValue != null && Regex.IsMatch(stringValue, Pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
