using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Extensions
{
    public static class ObjectExtension
    {
        public static T Cast<T>(this object objectToParse) where T: class
        {
            return objectToParse as T;
        }
    }
}
