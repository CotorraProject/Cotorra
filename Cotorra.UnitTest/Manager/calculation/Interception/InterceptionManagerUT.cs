using Cotorra.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Cotorra.UnitTest
{
   
    public class InterceptionManagerUT
    {
        public decimal Divide(decimal a, decimal b)
        {
            return a / b;
        }

        [Fact]
        public void GetInterception()
        {
            //104646-ZTGGJQQQATQEQWQCS6KX77JFD4UYQYRT4V6L6R6HCHKK2J77R4MJ5A4MS6KTGA4EFY5KJZP8MSBQE4BALYTCH9Y3ZALHGF5AQTWYZERZAUPA
            var result = Divide(10, 3);

        }
    }
}
