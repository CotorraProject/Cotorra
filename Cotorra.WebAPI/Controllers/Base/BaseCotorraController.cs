using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CotorraNode.Common.Service.Provisioning.API;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Newtonsoft.Json;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Cotorra.WebAPI.Controllers
{
    public class BaseCotorraController : BaseController, ICotorriaController
    {
        public Guid InstanceID { get; set; }

        internal Type GetType(string typeFullName)
        {
            return Assembly.GetAssembly(typeof(Employee)).GetType(typeFullName);
        }

        internal object InvokeMethod(string typeFullName, string methodName, params object[] parameters)
        {
            var type = GetType(typeFullName);
            var genericType = typeof(MiddlewareManager<>).MakeGenericType(type);
            var baseRecordManagerType = typeof(BaseRecordManager<>).MakeGenericType(type);
            var baseRecordManagerInstance = Activator.CreateInstance(baseRecordManagerType);
            var validator = new ValidatorFactory().CreateInstance(type);
            var instance = Activator.CreateInstance(genericType, new object[] {
                    baseRecordManagerInstance, validator });
            MethodInfo methodG = genericType.GetMethod(methodName);
            var result = methodG.Invoke(instance, parameters);

            return result;
        }

        internal async Task<object> InvokeMethodAsync(string typeFullName, string methodName, params object[] parameters)
        {
            var type = GetType(typeFullName);
            var genericType = typeof(MiddlewareManager<>).MakeGenericType(type);
            var baseRecordManagerType = typeof(BaseRecordManager<>).MakeGenericType(type);
            var baseRecordManagerInstance = Activator.CreateInstance(baseRecordManagerType);
            var validator = new ValidatorFactory().CreateInstance(type);
            var instance = Activator.CreateInstance(genericType, new object[] {
                    baseRecordManagerInstance, validator });
            MethodInfo methodG = genericType.GetMethod(methodName);
            var task = (Task)methodG.Invoke(instance, parameters);

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        internal object InvokeBooleanExpression(string typeFullName, string expressionNode)
        {
            var type = GetType(typeFullName);
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());
            var expression = serializer.DeserializeText(expressionNode).ToExpressionNode();
            MethodInfo method = typeof(ExpressionNode).GetMethod("ToBooleanExpression");
            MethodInfo generic = method.MakeGenericMethod(type);
            var booleanExpression = generic.Invoke(expression, new object[] { null });

            return booleanExpression;
        }

        internal object GetGenericList(string typeFullName, string jsonListObjects)
        {
            var typeList = typeof(List<>).MakeGenericType(GetType(typeFullName));
            var listConverted = JsonConvert.DeserializeObject(jsonListObjects, typeList);

            return listConverted;
        }

        internal object GetGenericArray(Type type, string jsonListObjects)
        {
            var typeList = type.MakeArrayType();
            var listConverted = JsonConvert.DeserializeObject(jsonListObjects, typeList);

            return listConverted;
        }

        internal object GetGenericListPrimitive(string typeFullName, string jsonListObjects)
        {
            var typeList = typeof(List<>).MakeGenericType(Type.GetType(typeFullName));
            var listConverted = JsonConvert.DeserializeObject(jsonListObjects, typeList);

            return listConverted;
        }
    }
}
