using CotorraNode.Common.ManageException;
using System;

namespace Cotorra.DigitalSign
{
    public class DigitalSignException : BaseException
    {
        public DigitalSignException(int errorCode, string code, string message, Exception innerException)
            : base(errorCode, code, message, innerException)
        {

        }

        public DigitalSignException(int errorCode, string code, string message)
            : base(errorCode, code, message, null)
        {

        }

    }
}
