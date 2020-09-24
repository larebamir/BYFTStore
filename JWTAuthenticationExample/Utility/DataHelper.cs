using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;


namespace JWTAuthenticationExample.Utility
{
    public static class DataHelper
    {

        public static bool IsEmptyRow(this DataRow dr)
        {
            var IsEmpty = false;

            foreach (object item in dr.ItemArray)
            {
                if (!(item is DBNull) || !string.IsNullOrWhiteSpace(item as string))
                {
                    IsEmpty = true;
                    break;
                }
            }

            return IsEmpty;
        }

        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {

                    if (pro.Name == column.ColumnName && dr[column.ColumnName] != System.DBNull.Value)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }


        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;

                t.Rows.Add(row);
            }

            return ds;
        }

        public static DataTable ToDataTable<T>(this T model, string tableName = null)
        {
            Type elementType = typeof(T);
            DataTable dt = new DataTable();

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                dt.Columns.Add(propInfo.Name, ColType);
            }

            DataRow row = dt.NewRow();

            //go through each property on T and add each value to the table
            foreach (var propInfo in elementType.GetProperties())
                row[propInfo.Name] = propInfo.GetValue(model, null) ?? DBNull.Value;

            dt.Rows.Add(row);

            if (!string.IsNullOrEmpty(tableName))
                dt.TableName = tableName;

            return dt;
        }

        public static DataTable ToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                //table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item);

                table.Rows.Add(values);
            }

            return table;
        }



        public static DataTable CsvToDataTable(string[] rows)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < rows.Count(); i++)
            {
                string bulder = rows[i];
                IEnumerable<string> Col = SplitCSV(bulder);

                if (i == 0)
                {
                    int n = 0;


                    foreach (var cell in Col)
                    {
                        dt.Columns.Add();
                        dt.Columns[n].ColumnName = cell;
                        n++;
                    }
                }
                else
                {
                    dt.Rows.Add();
                    int m = 0;
                    foreach (var cell in Col)
                    {
                        if (!string.IsNullOrEmpty(cell))
                            dt.Rows[dt.Rows.Count - 1][m] = cell.TrimEnd('\r');

                        m++;
                    }
                }
            }
            return dt;
        }
        public static IEnumerable<string> SplitCSV(string line)
        {
            var s = new StringBuilder();
            bool escaped = false, inQuotes = false;
            foreach (char c in line)
            {
                if (c == ',' && !inQuotes)
                {
                    yield return s.ToString();
                    s.Clear();
                }
                else if (c == '\\' && !escaped)
                {
                    escaped = true;
                }
                else if (c == '"' && !escaped)
                {
                    inQuotes = !inQuotes;
                }
                else
                {
                    escaped = false;
                    s.Append(c);
                }
            }
            yield return s.ToString();
        }

    }
}