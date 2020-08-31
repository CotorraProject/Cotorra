using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Cotorra.Core.Extensions;
using System.Transactions;
using NPOI.OpenXmlFormats.Dml.Diagram;
using CotorraNode.Security.Policy.Expression.Expressions.Formulas;

namespace Cotorra.UnitTest
{
    public class FiscalStringsUtilsUT
    {
        

        [Fact]
        public static void Should_Generate_Different()
        {
            var res = FiscalStringsUtils.GenerateRFCs(100); 
            var distinct = res.Distinct(); 
            Assert.Equal(distinct.Count(), res.Count());
            
        }

        [Fact]
        public static void Should_Generate_Different_CURPS()
        {
            var res = FiscalStringsUtils.GenerateCURPs(100);
            var distinct = res.Distinct();
            Assert.Equal(distinct.Count(), res.Count());

        }

        [Fact]
        public static void Should_Generate_Different_NSS()
        {
            var res = FiscalStringsUtils.GenerateCURPs(100);
            var distinct = res.Distinct();
            Assert.Equal(distinct.Count(), res.Count());

        }

    

    }
}
