﻿using System.ComponentModel.DataAnnotations;

namespace AutoMed_Backend.Validators
{
    public class NumericNonNegativeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(Convert.ToInt32(value) < 0) return false; return true;
        }
    }
}
