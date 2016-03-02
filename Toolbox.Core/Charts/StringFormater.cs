using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Charts
{
    public class StringFormater
    {
        public FormatType FormatType { get; set; }
        public int DecimalPlaces { get; set; }
        public bool UseThousandSeparator { get; set; }

        public StringFormater()
        {
            FormatType = FormatType.Text;
            DecimalPlaces = 0;
            UseThousandSeparator = false;
        }

        public string Format(object value)
        {
            if (value == null)
                return String.Empty;

            string stringValue = value.ToString();
            double doubleValue = 0;
            if (!Double.TryParse(stringValue, out doubleValue))
                return stringValue;

            string format = String.Empty;
            switch (FormatType)
            {
                case FormatType.Text:
                    return stringValue;

                case FormatType.Number:
                    if (UseThousandSeparator)
                        format = "N";
                    else
                        format = "F";

                    break;

                case FormatType.Percent:
                    format = "P";
                    break;
            }
            format += DecimalPlaces;
            return String.Format("{0:" + format + "}", doubleValue);
        }
    }

    public enum FormatType
    {
        Text,
        Number,
        Percent
    }
}
