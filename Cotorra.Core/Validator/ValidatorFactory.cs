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
    public class ValidatorFactory : IValidatorFactory
    {

        public ValidatorFactory()
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
        public IValidator<T> CreateInstance<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            var derivedType = typeof(IValidator<>).MakeGenericType(type);
            var assembly = Assembly.GetAssembly(typeof(ValidatorFactory));

            var types = FindAllDerivedTypes(assembly, derivedType);

            return Activator.CreateInstance(types.FirstOrDefault()) as IValidator<T>;

        }

        public object CreateInstance(Type type)
        {
            var derivedType = typeof(IValidator<>).MakeGenericType(type);
            var assembly = Assembly.GetAssembly(typeof(ValidatorFactory));

            var types = FindAllDerivedTypes(assembly, derivedType);

            return Activator.CreateInstance(types.FirstOrDefault());

        }
    }
}
