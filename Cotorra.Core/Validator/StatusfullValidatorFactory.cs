using CotorraNode.Common.Base.Schema;
using Cotorra.Core.Interface;
using Cotorra.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class StatusfullValidatorFactory : IStatusfullValidatorFactory
    {

        public StatusfullValidatorFactory()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="derivedType"></param>
        /// <returns></returns>
        public List<Type> FindAllDerivedTypes(Assembly assembly, Type derivedType)
        {
            return assembly
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    derivedType.IsAssignableFrom(t)
                    ).ToList();
        }

        /// <summary>
        /// Validate is a List
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="toValidate"></param>
        /// <returns></returns>
        public IStatusFullValidator<T> CreateInstance<T>() where T : StatusIdentityCatalogEntityExt
        {
            Type type = typeof(T);
            var derivedType = typeof(IStatusFullValidator<>).MakeGenericType(type);
            var assembly = Assembly.GetAssembly(typeof(ValidatorFactory));

            var types = FindAllDerivedTypes(assembly, derivedType);

            return Activator.CreateInstance(types.FirstOrDefault()) as IStatusFullValidator<T>;

        }

        private object CreateInstance(Type type)
        {
            var derivedType = typeof(IStatusFullValidator<>).MakeGenericType(type);
            var assembly = Assembly.GetAssembly(typeof(ValidatorFactory));

            var types = FindAllDerivedTypes(assembly, derivedType);

            return Activator.CreateInstance(types.FirstOrDefault());

        }
    }
}
