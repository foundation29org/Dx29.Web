using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Dx29.Data.Resources;

namespace Dx29.Data
{
    public class SymptomTimeline
    {
        public SymptomTimeline()
        {
            Items = new List<SymptomTimelineItem>();
        }

        public IList<SymptomTimelineItem> Items { get; set; }

        public bool IsValid()
        {
            return !Items.Any(r => !r.IsValid());
        }
    }

    public class SymptomTimelineItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        [DateAfterThan("BirthDate",ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "The_start_date_cannot_be_later_than_today_and_must_be_after_birthdate")]
        public DateTimeOffset? StartDate { get; set; } = null;

        [DateAfter2Than("StartDate", "BirthDate", ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "The_end_date_must_be_after_the_start_date_before_todays_date_and_after_birthdate")]
        public DateTimeOffset? EndDate { get; set; } = null;

        public bool IsCurrent { get; set; } = false;

        [StringLength(250, ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "Notes_must_be_at_max_250_characters_long")]
        public string Notes { get; set; } = null;

        public DateTimeOffset? BirthDate { get; set; }

        public bool IsValid()
        {
            if (StartDate != null)
            {
                //if ((StartDate > DateTimeOffset.UtcNow) || (StartDate < BirthDate))
                if ((StartDate > DateTimeOffset.UtcNow))
                {
                    return false;
                }
            }
            if(EndDate != null)
            {
                //if ((EndDate > DateTimeOffset.UtcNow) || (EndDate < BirthDate))
                if ((EndDate > DateTimeOffset.UtcNow))
                {
                    return false;
                }
            }
            if (StartDate != null && EndDate != null)
            {
                if (StartDate > EndDate)
                {
                    return false;
                }
            }
            if (Notes?.Length > 250)
            {
                return false;
            }
            return true;
        }
    }

    public class DateAfterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateAfterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTimeOffset?)value;
            var currentDate = DateTimeOffset.Now;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTimeOffset?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue != null)
            {
                if (currentValue > currentDate)
                    return new ValidationResult(ErrorMessage);
                //if(currentValue < comparisonValue)
                    //return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

    public class DateAfter2ThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty1;
        private readonly string _comparisonProperty2;
        public DateAfter2ThanAttribute(string comparisonProperty1, string comparisonProperty2)
        {
            _comparisonProperty1 = comparisonProperty1;
            _comparisonProperty2 = comparisonProperty2;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTimeOffset?)value;
            var currentDate = DateTimeOffset.Now;
            var property1 = validationContext.ObjectType.GetProperty(_comparisonProperty1);
            var property2 = validationContext.ObjectType.GetProperty(_comparisonProperty2);

            if (property1 == null)
                throw new ArgumentException("Property with this name not found");
            if (property2 == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue1 = (DateTimeOffset?)property1.GetValue(validationContext.ObjectInstance);
            var comparisonValue2 = (DateTimeOffset?)property2.GetValue(validationContext.ObjectInstance);

            if (currentValue != null)
            {
                //if ((currentValue < comparisonValue1) || (currentValue > currentDate) || (currentValue < comparisonValue2))
                if ((currentValue < comparisonValue1) || (currentValue > currentDate))
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
