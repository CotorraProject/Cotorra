using CotorraNode.Common.Library.Public;
using CotorraNode.Common.ManageException;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public class MiddlewareManagerExceptions : BaseException
    {
        ///Constructor
        public MiddlewareManagerExceptions() { }

        ///Constructor with exception code and message
        /// For easing exception handling you can define a code for you exceptions
        public MiddlewareManagerExceptions(string code, string message) : base(code, message) { }

        ///Constructor with exception code, message and inner exception
        public MiddlewareManagerExceptions(string code, string message, Exception innerException) : base(code, message, innerException) { }

        ///Constructor with code, exception serialized information and context.
        protected MiddlewareManagerExceptions(string code, SerializationInfo info, StreamingContext context) : base(code, info, context) { }

        public MiddlewareManagerExceptions(int errorCode, string code, string message, Exception innerException) : base(errorCode, code, message, innerException) { }

        ///Exception base message
        public override string baseMessage { get { return "Ocurrió un error en el contabilizador"; } }


        public override BaseException getDefault(MethodBase methodBase, Exception innerException)
        {
            return new MiddlewareManagerExceptions(HashUtils.getHash(methodBase), this.baseMessage, innerException);
        }


        public override BaseException getDefault(MethodBase methodBase, string message)
        {
            return new MiddlewareManagerExceptions(HashUtils.getHash(methodBase), message);
        }


        public override BaseException getDefault(MethodBase methodBase)
        {
            return new MiddlewareManagerExceptions(HashUtils.getHash(methodBase), this.baseMessage);
        }
    }
}
