using System;

namespace System.ComponentModel.DataAnnotations
{
    public class IsTrueAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <returns>
        /// true if the specified value is valid; otherwise, false. 
        /// </returns>
        /// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.</param>
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value.GetType() != typeof(bool)) throw new InvalidOperationException("can only be used on boolean properties.");
            return (bool)value;
        }
    }

    public class IsTrueConditionalAttribute : ValidationAttribute
    {
        public IsTrueConditionalAttribute(string property, string expected)
        {
            Property = property;
            Expected = expected;
        }

        public override bool RequiresValidationContext => true;

        public string Property { get; }
        public string Expected { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string current = validationContext.ObjectType.GetProperty(Property).GetValue(validationContext.ObjectInstance) as String;
                if (current == Expected)
                {
                    if ((bool)value)
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
