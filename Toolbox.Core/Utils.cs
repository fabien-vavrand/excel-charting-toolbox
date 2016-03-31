using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Toolbox
{
    public static class Utils
    {
        #region Arrays
        public static void ResizeArray<T>(ref T[,] original, int x, int y)
        {
            T[,] newArray = (T[,])Array.CreateInstance(
                typeof(T), 
                new int[] { x, y },
                new int[] { original.GetLowerBound(0), original.GetLowerBound(1) });
            int minX = Math.Min(original.GetLength(0), newArray.GetLength(0));
            int minY = Math.Min(original.GetLength(1), newArray.GetLength(1));

            for (int i = 0; i < minX; ++i)
                Array.Copy(
                    original, 
                    original.GetLowerBound(1) + i * original.GetLength(1), 
                    newArray,
                    newArray.GetLowerBound(1) +  i * newArray.GetLength(1),
                    minY);

            original = newArray;
        }

        public static void Concatenate<T>(ref T[,] original, T[,] add)
        {
            int x = Math.Min(original.GetLength(0), add.GetLength(0));
            int y = original.GetLength(1) + add.GetLength(1);

            T[,] newArray = (T[,])Array.CreateInstance(
                typeof(T),
                new int[] { x, y },
                new int[] { original.GetLowerBound(0), original.GetLowerBound(1) });

            for (int i = 0; i < x; i++)
            {
                Array.Copy(
                    original, RowIndex(original, i),
                    newArray, RowIndex(newArray, i),
                    original.GetLength(1));
                Array.Copy(
                    add, RowIndex(add, i),
                    newArray, RowIndex(newArray, i) + original.GetLength(1),
                    add.GetLength(1));
            }
            original = newArray;
        }

        private static int RowIndex<T>(T[,] original, int i)
        {
            return original.GetLowerBound(1) + i * original.GetLength(1);
        }
        #endregion

        #region Enum
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
        #endregion
    }
}
