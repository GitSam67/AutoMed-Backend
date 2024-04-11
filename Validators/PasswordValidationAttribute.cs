using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AutoMed_Backend.Validators
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is string password)
            {
                int minLength = 8; 
                int maxLength = 20; 
                string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$";

                if (password.Length >= minLength && password.Length <= maxLength && Regex.IsMatch(password, pattern))
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
