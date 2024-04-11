using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Validators
{
    public class ExpiryDateValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is DateTime expiryDate)
            {
                if(expiryDate > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
