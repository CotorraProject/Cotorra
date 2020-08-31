using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class AuthorizationParams : IdentityWorkParams
    {
        public Guid user { get; set; }

        public Guid InstanceID { get; set; }

        public Guid PeriodDetailIDToAuthorize { get; set; }
    }
}
