using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox
{
    public static class Extensions
    {
        #region Numerics
        public static double Floor(this double value, double min)
        {
            return Math.Max(value, min);
        }

        public static double CapFloor(this double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
        #endregion

        #region String
        public static double ToDouble(this string value)
        {
            if (value == null)
                return 0;

            double d;
            Double.TryParse(value, out d);
            return d;
        }

        public static bool IsDouble(this string value)
        {
            if (value == null)
                return false;

            double d;
            return Double.TryParse(value, out d);
        }
        #endregion

        #region Bool
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
        #endregion

        #region Object
        public static double ToDouble(this object value)
        {
            if (value == null)
                return 0;

            return value.ToString().ToDouble();
        }

        public static bool IsDouble(this object value)
        {
            if (value == null)
                return false;

            return value.ToString().IsDouble();
        }

        public static T ConvertTo<T>(this object value)
        {
            if (value == null)
                return default(T);

            if (value is T)
                return (T)value;
            else if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(value.ToString(), typeof(T));
            else if (typeof(T) == typeof(double))
                return (T)Convert.ChangeType(value.ToDouble(), typeof(T));
            else
                return default(T);
        }
        #endregion

        #region Linq
        public static double Percentile(this IEnumerable<double> sequence, double percentile)
        {
            if (percentile < 0 || percentile > 1)
                throw new ArgumentException("Percentile should be between 0 and 1");
            
            var array = sequence.OrderBy(d => d).ToArray();
            int n = array.Length;
            double index = (n - 1) * percentile + 1;
            if (index == 1d) 
                return array[0];
            else if (index == n) 
                return array[n - 1];
            else
            {
                 int k = (int)index;
                 double d = index - k;
                 return array[k - 1] + d * (array[k] - array[k - 1]);
            }
        }
        #endregion
    }
}
