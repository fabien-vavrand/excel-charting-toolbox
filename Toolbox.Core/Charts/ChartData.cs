using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Charts
{
    public class ChartData
    {
        public object[,] Values { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public List<string> ColumnNames { get; set; }
        public bool HasHeader { get; set; }

        public ChartData(object[,] values)
        {
            Values = values;
            Columns = Values.GetLength(1);
            ComputeColumnNames();
            Rows = Values.GetLength(0);
            if (HasHeader)
                Rows--;
        }

        private void ComputeColumnNames()
        {
            for (int i = 1; i <= Columns; i++)
                if (Values[1, i] is double || Values[1, i] is DateTime)
                {
                    ColumnNames = Enumerable.Range(1, Columns).Select(n => "Column " + n).ToList();
                    HasHeader = false;
                    return;
                }

            ColumnNames = Enumerable.Range(1, Columns).Select(n => (Values[1, n] ?? "Column " + n).ToString()).ToList();
            HasHeader = true;
        }

        public T[] GetValues<T>(int column)
        {
            var result = new T[Rows];

            for (int i = 0; i < Rows; i++)
                result[i] = (T)Values.GetValue(i + 1, column);

            return result;
        }

        public List<T> GetValues<T>(string column)
        {
            var result = new T[Rows];

            for (int i = 0; i < Rows; i++)
                result[i] = Values.GetValue(i + 1 + HasHeader.ToInt(), ColumnNames.IndexOf(column) + 1).ConvertTo<T>();

            return result.ToList();
        }
    }
}
