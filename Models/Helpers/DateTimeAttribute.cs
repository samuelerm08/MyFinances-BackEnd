using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SistemaFinanciero.WebApi.Models.Helpers
{
    public class DateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {            

            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            bool formatoFechaValido = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime fechaValida);

            if (formatoFechaValido)
            {
                return new ValidationResult("Formato de fecha inválido. El formato debe ser: 'yyyy-MM-dd'");
            }

            return ValidationResult.Success;
        }
    }
}
