using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Validators
{
    public class ContactFormatValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string contactNumber) {
                string formatNumber = new string(contactNumber.Where(char.IsDigit).ToArray());

                if (formatNumber.Length == 10)
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
