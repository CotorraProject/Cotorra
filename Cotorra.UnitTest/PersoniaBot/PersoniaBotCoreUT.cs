using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Public;
using Cotorra.Client;
using Cotorra.Core;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Cotorra.UnitTest
{
    public class CotorriaBotCoreUT
    {

        [Theory]
        [InlineData("vacaciones los dias 23 de enero y 5 de febrero")]
        [InlineData("vacaciones los días 21, 22, 23, 26 de enero y 1,2,3 de febrero 2020")]
        [InlineData("solicito vacaciones del 23 de febrero al 25 de febrero del 2020")]
        [InlineData("quiero vacaciones el dia 23 de junio de 2020")]
        public async Task GetVacationsIntent(string utterance)
        {
            CotorriaBotManager CotorriaBotManager = new CotorriaBotManager(new CotorriaBotLUISProvider());
            var res = await CotorriaBotManager.GetIntent(utterance);

            Assert.NotNull(res);
            Assert.NotEmpty(res);            

        }


        [Theory]
        [InlineData("vacaciones los dias 23 de enero y 5 de febrero")]
        [InlineData("vacaciones los días 21, 22, 23, 26 de enero y 1,2,3 de febrero 2020")]
        [InlineData("solicito vacaciones del 23 de febrero al 25 de febrero del 2020")]
        [InlineData("quiero vacaciones el dia 23 de junio de 2020")]
        public async Task GetVacationsIntentClient(string utterance)
        {
            CotorriaBotClient client = new CotorriaBotClient("", ClientConfiguration.ClientAdapter.Proxy);

            var res = await client.GetIntent(new GetIntentParams()
            {
                Utterance = utterance
            }); 

            Assert.NotNull(res);
            Assert.NotEmpty(res);

        }




    }
}
