using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dx29.Web.Areas.Identity.Pages
{
    public class PasswordDigitAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (string)value;
            string patternNumber = @"^(?=.*[0-9]).{6,100}$";
            if (currentValue != null)
            {
                if (Regex.IsMatch(currentValue, patternNumber) == false)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-passwordDigit", errorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
            {
                if (attributes.ContainsKey(key))
                {
                    return false;
                }
                attributes.Add(key, value);
                return true;
            }
    }

    public class PasswordLowerCaseAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (string)value;
            string patternLowerCase = @"^(?=.*[a-z]).{6,100}$";

            if (currentValue != null)
            {
                if (Regex.IsMatch(currentValue, patternLowerCase) == false)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-passwordLower", errorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }

    public class PasswordUpperCaseAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (string)value;
            string patternUpperCase = @"^(?=.*[A-Z]).{6,100}$";
            if (currentValue != null)
            {
                if (Regex.IsMatch(currentValue, patternUpperCase) == false)
                {
                    return new ValidationResult(ErrorMessage);
                }
                
            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-passwordUpper", errorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }

    public class PasswordSpecialCaseAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (string)value;
            string patternSpecial = @"^(?=.*[^a-zA-Z\d]).{6,100}$";

            if (currentValue != null)
            {
                if (Regex.IsMatch(currentValue, patternSpecial) == false)
                {
                    return new ValidationResult(ErrorMessage);
                }

            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-passwordSpecial", errorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }

    public class PasswordLenghtAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (string)value;
            int minLenght = 6;
            int maxLength = 100;

            if (currentValue != null)
            {
                if (currentValue.Length< minLenght)
                {
                    return new ValidationResult(ErrorMessage);
                }
                if(currentValue.Length > maxLength)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-passwordLenght", errorMessage);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }

}
