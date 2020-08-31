using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public interface IStatusFull
    {
        public CotorriaStatus LocalStatus { get; set; }
        public DateTime LastStatusChange { get; set; }
    }

    public enum CotorriaStatus
    {
        Active = 0,
        Unregistered = 1,
        Inactive = 2,
    }
}
