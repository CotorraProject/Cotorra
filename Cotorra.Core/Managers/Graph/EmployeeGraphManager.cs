using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Graph
{
    public class EmployeeGraphManager
    {
        /// <summary>
        /// Crea un nodo del empleado como tabla de grafo en SQL.
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="InstanceId"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task CreateEmployeeNodeAsync(List<Employee> employees)
        {
            if (employees.Any())
            {
                //Instancia y compañia
                Guid InstanceId = employees.FirstOrDefault().InstanceID;
                Guid company = employees.FirstOrDefault().company;

                var employeesIds = employees.Select(p => p.ID).ToList();
                using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CreateEmployeesNodes";

                        DataTable dtGuidList = new DataTable();
                        dtGuidList.Columns.Add("ID", typeof(string));
                        employeesIds.ForEach(p =>
                        {
                            dtGuidList.Rows.Add(p);
                        });
                        SqlParameter param = new SqlParameter("@EmployeesIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.guidlisttabletype",
                            Value = dtGuidList
                        };
                        command.Parameters.Add(param);
                        command.Parameters.AddWithValue("@InstanceId", InstanceId);
                        command.Parameters.AddWithValue("@company", company);

                        //Execute SP de autorización
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}
