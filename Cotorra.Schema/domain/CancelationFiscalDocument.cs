using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum CancelationFiscalDocumentStatus
    {
        None = 0,
        ErrorInRequest = 1,
        Requested = 2,
        Done = 3,
        ErrorInResponse = 4
    }

    [DataContract]
    public class CancelationFiscalDocument : IdentityCatalogEntityExt
    {
        public CancelationFiscalDocument()
        {
            CancelationFiscalDocumentDetails = new List<CancelationFiscalDocumentDetail>();
        }

        [DataMember]
        public Guid? CancelationRequestXMLID { get; set; }

        [DataMember]
        public Guid? CancelationResponseXMLID { get; set; }

        [DataMember]
        public virtual List<CancelationFiscalDocumentDetail> CancelationFiscalDocumentDetails { get; set; }
    }

    [DataContract]
    public class CancelationFiscalDocumentDetail : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid CancelationFiscalDocumentID { get; set; }

        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public CancelationFiscalDocumentStatus CancelationFiscalDocumentStatus { get; set; }

        [DataMember]
        public virtual CancelationFiscalDocument CancelationFiscalDocument { get; set; }
    }
}
