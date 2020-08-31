using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class AuthorizationByOverdraftParams : IdentityWorkParams
    {
        public Guid user { get; set; }

        public Guid InstanceID { get; set; }

        public List<Guid> OverdraftIDs { get; set; }
    }
}
