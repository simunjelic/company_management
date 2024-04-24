using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Validation
{
    public class EndDateAfterStartDateAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public EndDateAfterStartDateAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDateProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);

            if (startDateProperty == null || endDateProperty == null)
            {
                return new ValidationResult($"Invalid property names: {validationContext.MemberName} or {_startDatePropertyName}");
            }

            var endDateValue = (DateTime?)endDateProperty.GetValue(validationContext.ObjectInstance, null);
            var startDateValue = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance, null);

            if (endDateValue.HasValue && startDateValue.HasValue && endDateValue.Value <= startDateValue.Value)
            {
                return new ValidationResult(ErrorMessage ?? "Datum od mora biti prije datuma do.");
            }

            return ValidationResult.Success;
        }
    }
}
