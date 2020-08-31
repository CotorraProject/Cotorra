using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Interface
{
    public interface IStatusfullValidatorFactory
    {
        IStatusFullValidator<T> CreateInstance<T>() where T : StatusIdentityCatalogEntityExt;

        List<Type> FindAllDerivedTypes(Assembly assembly, Type derivedType);
    }
}
