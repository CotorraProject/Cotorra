using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public interface IBuilder<T>
    {
        T Build();
    }
}
