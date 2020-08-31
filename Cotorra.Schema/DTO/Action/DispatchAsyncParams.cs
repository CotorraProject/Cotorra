using CotorraNode.CommonApp.Schema;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class DispatchAsyncParams
    {
        [DataMember]
        public Guid ActionID { get; set; }

        [DataMember]
        public Guid RegisterID { get; set; }

    }
}
