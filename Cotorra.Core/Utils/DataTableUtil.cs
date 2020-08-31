using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Utils
{
    public static class DataTableUtil
    {
        public static List<T> ConvertDataTable<T>(DataTable dt)
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
                    if (pro.Name == column.ColumnName)
                    {
                        var value = dr[column.ColumnName];
                        if (DBNull.Value != value)
                        {
                            pro.SetValue(obj, value, null);
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
