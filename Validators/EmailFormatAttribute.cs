using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AutoMed_Backend.Validators
{
    public class EmailFormatAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string email)
            {
                string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

                if (Regex.IsMatch(email, emailPattern))
                {
                    return true;
                }
                else
                {
                    return false;
                } 
            }
            return false;
        }
    }
}
