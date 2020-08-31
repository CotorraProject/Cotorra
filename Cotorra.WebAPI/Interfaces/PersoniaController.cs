using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    interface ICotorriaController
    {
        public Guid InstanceID { get; set; }
    }
}
