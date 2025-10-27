using System.ComponentModel.DataAnnotations;
using HomeCareAppointment.Models;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AvailableDayRangeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is AvailableDay AvailableDay)
        {
            if (AvailableDay.StartTime >= AvailableDay.EndTime)
            {
                return new ValidationResult("Start time cannot be after end time.",
                    new[] { nameof(AvailableDay.StartTime), nameof(AvailableDay.EndTime) });
            }
        }
        return ValidationResult.Success;
    }
}