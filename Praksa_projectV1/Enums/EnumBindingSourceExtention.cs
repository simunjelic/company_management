using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Praksa_projectV1.Enums
{
    public class EnumBindingSourceExtention : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtention(Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                throw new Exception("EnumType is null or not EnumType");
            }
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
