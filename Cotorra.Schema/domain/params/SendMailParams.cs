using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public enum TypeAttachment
    { 
        XML = 1,
        PDF = 2,
        TXT = 3
    }

    [Serializable]
    [DataContract]
    public class SendMailAddress
    { 
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SendMailAttachment
    {
        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public TypeAttachment TypeAttachment { get; set; }

        [DataMember]
        public byte[] Attachment { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SendMailParams
    {
        public SendMailParams()
        {
            SendMailAddresses = new List<SendMailAddress>();
            SendMailAttachments = new List<SendMailAttachment>();
        }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string PlainContentText { get; set; }

        [DataMember]
        public string HTMLContent { get; set; }

        [DataMember]
        public List<SendMailAddress> SendMailAddresses { get; set; }

        [DataMember]
        public List<SendMailAttachment> SendMailAttachments { get; set; }
    }
}
