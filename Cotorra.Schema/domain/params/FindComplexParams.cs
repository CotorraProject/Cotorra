using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum OperationType
    {
        Count = 1,
        Select = 2
    }

    [DataContract]
    [Serializable]
    public class FindComplexParams
    {
        [DataMember]
        public OperationType OperationType { get; set; }
        [DataMember]
        public Guid IdentityWorkId { get; set; }
        [DataMember]
        public Guid InstanceId { get; set; }
        [DataMember]
        public string TypeFullName { get; set; }
        [DataMember]
        public string ExpressionNode { get; set; }
        [DataMember]
        public string[] ObjectsToInclude { get; set; }
    }
}
