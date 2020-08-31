using Cotorra.Client;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cotorra.UnitTest
{
    public class SearchClientProxyUT
    {

        [Fact]
        public async Task GetEmployeeByIDentityUserIDProxyAsync()
        {

            SearchClientProxy client = new SearchClientProxy("");

            var query = "{  employeeIdentity(id: {instanceID: \"CDC79411-6988-4782-A7B8-2E515D6BF474\",companyID: " +
                "\"0C08DAA6-F775-42A8-B75E-1B9B685B7977\",  identityID:\"13AF40D9-1B28-138F-FCD9-0ECA25747940\" })" + "" +
                "  { code, fullName, departmentName   } } ";

            var res = await client.QueryAsync<EmployeeIdentityDTOGraphqE>(query);

            Assert.NotNull(res.EmployeeIdentity);
        }


        [Fact]
        public async Task GetEmployeeByIDProxyAsync()
        {

            SearchClientProxy client = new SearchClientProxy("");

            var query = "{  employee(id: {instanceID: \"CDC79411-6988-4782-A7B8-2E515D6BF474\",companyID: " +
                "\"0C08DAA6-F775-42A8-B75E-1B9B685B7977\",  id:\"F2D90173-E2FD-48DF-8358-07B66318810C\" })" + "" +
                "  { code, fullName, departmentName   } } ";

            var res = await client.QueryAsync<EmployeeDTOGraphql>(query);

            Assert.NotNull(res.Employee);
        }

        [Fact]
        public async Task GetEmployeeByID()
        {

            ISearchClient client = new SearchClient("", ClientConfiguration.ClientAdapter.Proxy);

            var query = "{   employee(id: {instanceID: \"CDC79411-6988-4782-A7B8-2E515D6BF474\",companyID: " +
                "\"0C08DAA6-F775-42A8-B75E-1B9B685B7977\",  id:\"F2D90173-E2FD-48DF-8358-07B66318810C\" })" + "" +
                "  { code, fullName, departmentName   } } ";

            var res = await client.QueryAsync<EmployeeDTOGraphql>(query);

            Assert.NotNull(res);
        }

        [Fact]
        public async Task GetEmployees()
        {

            ISearchClient client = new SearchClient("", ClientConfiguration.ClientAdapter.Proxy);

            var query = "{employees(instanceID: \"CDC79411-6988-4782-A7B8-2E515D6BF474\", companyID:" +
                " \"0C08DAA6-F775-42A8-B75E-1B9B685B7977\") {code, fullName, departmentName  }}";

            var res = await client.QueryAsync<EmployeesDTOGraphql>(query);

            Assert.NotNull(res);
        }

        [Fact]
        public async Task OverdrafstUsingConfigAsync()
        {

            ISearchClient client = new SearchClient("", ClientConfiguration.ClientAdapter.Proxy);

            var query = "{ overdraftEmployeeId   (id: {  instanceID: \"D1A73B60-AB4E-4E9C-AFAD-869E5C1E0B47\", companyID: \"0C08DAA6-F775-42A8-B75E-1B9B685B7977\",  employeeID:\"3947752A-8ECE-436D-8BA4-4F1F8828C57C\" }) { iD} }";

            var res = await client.QueryAsync<OverdraftsDTOGraphql>(query);

            Assert.NotNull(res);
        }

        [Fact]
        public async Task EmployeeBenefits()
        {

            ISearchClient client = new SearchClient("", ClientConfiguration.ClientAdapter.Proxy);

            var query = "{ employeeBenefits   (  instanceID: \"CDC79411-6988-4782-A7B8-2E515D6BF474\", companyID: \"0C08DAA6-F775-42A8-B75E-1B9B685B7977\",  employeeID:\"F2D90173-E2FD-48DF-8358-07B66318810C\" ) {  vacationalBonusPerSeniority, vacationsPerSeniority, yearlyBonusPerSeniority, pendingVacationDays, freeDays} }";

            var res = await client.QueryAsync<EmployeeBenefitsDTOGraphql>(query);

            Assert.NotNull(res);
        }

    }
}
