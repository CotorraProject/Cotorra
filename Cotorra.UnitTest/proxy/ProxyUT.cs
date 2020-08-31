using CotorraNode.Common.Config;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cotorra.UnitTest.proxy
{
    public class ProxyUT
    {
        public class XamarinConfigProvider : IConfigProvider
        {
            public dynamic GetAppSettings()
            {
                //throw new NotImplementedException();
                return null;
            }

            public string GetValue(string key)
            {
                var value = String.Empty;
                if (key == "CotorraService")
                {
                    value = "http://cotorraapi.azurewebsites.net/";
                }
                return value;
            }

            public void SetCurrentDomain(string domain)
            {
                //throw new NotImplementedException();
            }

            public bool ValidateProvider()
            {
                // throw new NotImplementedException();
                return true;
            }
        }

        [Fact]
        public async Task TM_TestClient()
        {
            try
            {
                var client = new ClientProxy<Employee>("", new XamarinConfigProvider());
                var companyID = Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977");
                var employees = await client.FindAsync(p => p.company == companyID, companyID);
            }
            catch { 
            
            }

        }
    }
}
