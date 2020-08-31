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
    public class GetIntentParams : IInstanceIDParams
    {
        [DataMember]
        public string Utterance { get; set; } 

    }
}
