﻿using Restaurant_Manager.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Restaurant_Manager.ValidationRules
{
    public class NumberValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string number)
            {
                string numberPattern = @"^[0-9]*$";
                if (Regex.IsMatch(number, numberPattern))
                {
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, $"Invalid number format");
        }

    }
}
