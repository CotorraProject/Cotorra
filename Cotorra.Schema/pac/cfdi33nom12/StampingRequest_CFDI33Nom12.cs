using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.pac
{
    [DataContract]
    public class CFDIRequest_CFDI33Nom12
    {
        [DataMember]
        public string XmlString { get; set; }
    }

    [DataContract]
    public class StampingRequest_CFDI33Nom12
    {
        public StampingRequest_CFDI33Nom12()
        {
            AdditionalInformation = new List<AdditionalInformation_CFDI33Nom12>();
        }

        [DataMember]
        [JsonProperty("PACDocumentType")]
        public string PACDocumentType { get; set; }

        [DataMember]
        [JsonProperty("Token")]
        public string Token { get; set; }

        [DataMember]
        [JsonProperty("Body")]
        public CFDIRequest_CFDI33Nom12 Body { get; set; }

        [DataMember]
        [JsonProperty("Version")]
        public string Version { get; set; }

        [DataMember]
        [JsonProperty("AdditionalInformation")]
        public List<AdditionalInformation_CFDI33Nom12> AdditionalInformation { get; set; }
    }

    [DataContract]
    public class StampingResult_CFDI33Nom12
    {
        public StampingResult_CFDI33Nom12()
        {
            ResponseList_CFDI33Nom12 = new List<ResponseList_CFDI33Nom12>();
        }

        [DataMember]
        [JsonProperty("ResultType")]
        public string ResultType { get; set; }

        [DataMember]
        [JsonProperty("ResponseList")]
        public List<ResponseList_CFDI33Nom12> ResponseList_CFDI33Nom12 { get; set; }
    }

    [DataContract]
    public class ResponseList_CFDI33Nom12
    {
        [JsonProperty("ResponseType")]
        [DataMember]
        public string ResponseType { get; set; }

        [JsonProperty("ResponseValue")]
        [DataMember]
        public string ResponseValue { get; set; }
    }

    [DataContract]
    public class AdditionalInformation_CFDI33Nom12
    {
        public AdditionalInformation_CFDI33Nom12() { }

        public AdditionalInformation_CFDI33Nom12(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        [JsonProperty("Key")]
        public string Key { get; set; }

        [DataMember]
        [JsonProperty("Value")]
        public string Value { get; set; }


    }
}
