using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MyFinances.WebApi.Models.Helpers
{
    public class DateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {            

            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            bool isDateFormatValid = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime validDate);

            if (isDateFormatValid)
            {
                return new ValidationResult("Invalid date format. The format should be: 'yyyy-MM-dd'");
            }

            return ValidationResult.Success;

        }
    }
}
