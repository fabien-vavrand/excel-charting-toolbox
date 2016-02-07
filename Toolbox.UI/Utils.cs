using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox
{
    public static class Utils
    {
        public static IEnumerable<KeyValuePair<TEnum, string>> EnumKeyValues<TEnum>()
        {
            return typeof(TEnum).GetEnumValues()
                                .Cast<TEnum>()
                                .Select(e => new KeyValuePair<TEnum, string>(e, e.AsString()));
        }

        public static string AsString<TEnum>(this TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) 
                return attributes[0].Description;
            else 
                return value.ToString();
        }
    }
}
