using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public interface IProcessValidator<T>
    {
        void BeforeProcess( T lstObjectsToValidate);

        
    }
}
