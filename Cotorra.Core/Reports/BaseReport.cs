using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Cotorra.Core.Reports
{
    public class BaseReport
    {
        public async Task<DataSet> ExecuteSPAsync(string spName,Dictionary<string, object> parameters)
        {
            DataSet dataSet = new DataSet();
            using (var conn = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                using (SqlDataAdapter da = new SqlDataAdapter(spName, conn))
                {
                    foreach (var parameter in parameters)
                    {
                        da.SelectCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.Fill(dataSet);
                }
                conn.Close();
            }

            return dataSet;
        }

        private string ConvertDataTableToJson(DataTable dataTable)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            var result = JsonConvert.SerializeObject(rows);
            return result;
        }

        public dynamic GetObjectFromDataTable(DataSet dataSet, int positionInDataSet)
        {
            var datatable = dataSet.Tables[positionInDataSet];
            return JsonConvert.DeserializeObject(ConvertDataTableToJson(datatable));
        }
    }
}
