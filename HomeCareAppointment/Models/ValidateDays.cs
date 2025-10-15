using System; 
using System.ComponentModel.DataAnnotations;

public class ValidateDays : ValidationAttribute //Start of validation, not done
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
        {
            return date >= DateTime.Today;
        }
        return true;
    }
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be today or future date.";
    }
}