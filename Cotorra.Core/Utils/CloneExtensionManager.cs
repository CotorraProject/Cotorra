using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Utils
{
    public class CloneExtensionManager<T>
    {


        public  T DeepClone(T toClone)
        {

            return toClone.DeepClone();

        }
    }
}
