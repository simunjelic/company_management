using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Validation
{
    public class SecureStringAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public SecureStringAttribute(int minLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is SecureString secureString)
            {
                if (GetSecureStringLength(secureString) < _minLength)
                {
                    return new ValidationResult($"Polje mora sadržavati barem {_minLength} znakova.");
                }
            }
            else
            {
                return new ValidationResult("Polje ne može biti prazno.");
            }

            return ValidationResult.Success;
        }

        private int GetSecureStringLength(SecureString secureString)
        {
            int length = 0;
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
                while (System.Runtime.InteropServices.Marshal.ReadInt16(ptr, length * 2) != 0)
                {
                    length++;
                }
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
                }
            }

            return length;
        }
    }
}
