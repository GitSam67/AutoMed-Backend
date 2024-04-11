using AutoMed_Backend.Validators;
using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Models
{
    public class CashBalance
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        [NumericNonNegative(ErrorMessage ="Balance must be numeric and non negative")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "TotalSales is required")]
        [NumericNonNegative(ErrorMessage = "Total sales must be numeric and non negative")]
        public decimal TotalSales { get; set; }

        [Required(ErrorMessage = "Branch Id is required")]
        [NumericNonNegative(ErrorMessage = "Branch id must be numeric and non negative")]
        public int BranchId { get; set; } 
    }
}
