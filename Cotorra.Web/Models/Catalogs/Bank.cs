using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Web.Models
{
    public class Bank
    {
        public Guid? ID { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public Int32 Code { get; set; }
    }
}
