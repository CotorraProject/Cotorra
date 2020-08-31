using CotorraNode.Common.Library.Public;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Utils
{
    static class CloneExtension
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static T DeepClone<T>(this T obj)
        {
            var txt = JsonSerializer.SerializeObject(obj);
            T newObj = JsonSerializer.DeserializeObject<T>(txt);
            //var txt = System.Text.Json.JsonSerializer.Serialize(obj);
            //T newObj = System.Text.Json.JsonSerializer.Deserialize<T>(txt);
            return newObj;
        }
    }
}
