using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Validation
{
    public class StartDateBeforeEndDateAttribute : ValidationAttribute
    {
        private readonly string _endDatePropertyName;

        public StartDateBeforeEndDateAttribute(string endDatePropertyName)
        {
            _endDatePropertyName = endDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var endDateProperty = validationContext.ObjectType.GetProperty(_endDatePropertyName);

            if (startDateProperty == null || endDateProperty == null)
            {
                return new ValidationResult($"Invalid property names: {validationContext.MemberName} or {_endDatePropertyName}");
            }

            var startDateValue = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance, null);
            var endDateValue = (DateTime?)endDateProperty.GetValue(validationContext.ObjectInstance, null);

            if (startDateValue.HasValue && endDateValue.HasValue && startDateValue.Value >= endDateValue.Value)
            {
                return new ValidationResult(ErrorMessage ?? "Datum od mora biti prije Datuma do.");
            }

            return ValidationResult.Success;
        }
    }
}
