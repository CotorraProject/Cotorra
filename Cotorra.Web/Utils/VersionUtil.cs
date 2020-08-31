using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Web.Utils
{
    public class VersionUtil
    {
        public static string GetUserOS()
        {
            var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            return osNameAndVersion;
        }
    }
}
