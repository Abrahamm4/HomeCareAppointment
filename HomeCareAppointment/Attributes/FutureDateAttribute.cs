using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCareAppointment.Attributes{
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
        {
            ErrorMessage = "Date must be today or a future date.";
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; //[Required] should handle it

            if (value is DateTime date)
            {
                if (date.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid date format.");
        }
    }
}