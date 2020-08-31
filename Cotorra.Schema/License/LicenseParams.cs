using CotorraNode.CommonApp.Schema;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class LicenseParams : SecurityLicensingParams, IParams
    {
        public LicenseParams() { }
         
        public Guid CotorriaAppID { get; set; }
        public Guid LicenseServiceID { get; set; }
         
    }

   

}
