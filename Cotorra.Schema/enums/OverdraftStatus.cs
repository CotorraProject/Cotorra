using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public enum OverdraftStatus
    {
        None = 0,
        Authorized = 1,
        Stamped = 2,

        Canceled = 99
    }
}
