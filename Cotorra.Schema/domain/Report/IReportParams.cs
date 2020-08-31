using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema.Report
{
    public interface IReportParams
    {
        public Guid company { get; set; }

        public Guid InstanceID { get; set; }

        public Guid user { get; set; }
    }
}
