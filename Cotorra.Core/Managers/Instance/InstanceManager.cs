using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Private;
using CotorraNode.Layer2.Company.Client;
using CotorraNode.Platform.Layer2.Company.Schema.Results;

namespace Cotorra.Core
{
    public class InstanceManager  
    {
         
        public InstanceManager( )
        {
            
        }

        public async Task<List<UserCompaniesResult>> GetAllAsync(string token)
        {
            InstanceClient instanceClient = new InstanceClient(ConfigManager.GetValue("CompanyHost"));
            var platInstances = await instanceClient.GetInstancesAsync(token, Guid.Parse(ConfigManager.GetValue("ServiceIDs")));
            return platInstances;
        }

        public async Task<InstanceResult> GetByIDAsync(string token, Guid instanceID)
        {
            InstanceClient instanceClient = new InstanceClient(ConfigManager.GetValue("CompanyHost"));
            var platInstance = await instanceClient.GetInstanceAsync(token, instanceID);
            return platInstance;
        }


    }

}
