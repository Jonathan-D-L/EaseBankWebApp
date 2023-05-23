using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Infrastructure.Validation
{
    public class DateAttribute : ValidationAttribute
    {

        public DateAttribute()
        {
            ErrorMessage = "Please select a birth date.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            if (DateTime.TryParse(value.ToString(), out DateTime d))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
