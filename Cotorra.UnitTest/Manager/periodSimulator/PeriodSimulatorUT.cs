using Microsoft.EntityFrameworkCore.Internal;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cotorra.UnitTest
{
    public class PeriodSimulatorUT
    {
        [Fact]
        public async Task PeriodSimulator_Should_Generate_Period_Details()
        {
            var periodSimulator = new PeriodSimulator();
            var details = periodSimulator.GetPeriodDetails(PaymentPeriodicity.Weekly, new DateTime(2019,12,23), 7, false);
            Assert.True(details.Any());
            Assert.True(details.Count == 53);

            details = periodSimulator.GetPeriodDetails(PaymentPeriodicity.Weekly, new DateTime(2019, 2, 25), 7, false);
            Assert.True(details.Any());
            Assert.True(details.Count == 44);

            details = periodSimulator.GetPeriodDetails(PaymentPeriodicity.Weekly, new DateTime(2019, 10, 14), 7, false);
            Assert.True(details.Any());
            Assert.True(details.Count == 63);
        }
    }
}
