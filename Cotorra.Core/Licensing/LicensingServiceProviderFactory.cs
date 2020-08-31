using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core
{
    public class LicensingServiceProviderFactory
    {
        public static ILicensingServiceProvider GetProvider()
        {
            return new LicensingServiceCotorraProvider();
        }
    }
}
