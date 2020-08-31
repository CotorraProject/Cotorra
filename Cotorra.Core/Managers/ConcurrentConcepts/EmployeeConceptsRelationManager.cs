using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class EmployeeConceptsRelationManager
    {
        public async Task CreateConcurrentConceptsAsync(Guid instanceID, Guid companyID, Guid user)
        {
            //AddConceptPayment_FromEmployeeConceptsRelation
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "AddConceptPayment_FromEmployeeConceptsRelation";
                    command.Parameters.AddWithValue("@InstanceId", instanceID);
                    command.Parameters.AddWithValue("@company", companyID);
                    command.Parameters.AddWithValue("@user", user);

                    //Execute SP
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
