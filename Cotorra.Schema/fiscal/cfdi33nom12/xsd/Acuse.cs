using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace Cotorra.Schema.CFDI33Nom12
{
    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://tempuri.org/")]
    [Serializable]
    public class Acuse
    {
        private Folios[] foliosField;
        private SignatureType signatureField;
        private string rfcEmisorField;
        private DateTime fechaField;
        private string codEstatusField;

        [XmlElement("Folios", Namespace = "http://cancelacfd.sat.gob.mx")]
        public Folios[] Folios
        {
            get
            {
                return this.foliosField;
            }
            set
            {
                this.foliosField = value;
            }
        }

        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        [XmlAttribute]
        public string RfcEmisor
        {
            get
            {
                return this.rfcEmisorField;
            }
            set
            {
                this.rfcEmisorField = value;
            }
        }

        [XmlAttribute]
        public DateTime Fecha
        {
            get
            {
                return this.fechaField;
            }
            set
            {
                this.fechaField = value;
            }
        }

        [XmlAttribute]
        public string CodEstatus
        {
            get
            {
                return this.codEstatusField;
            }
            set
            {
                this.codEstatusField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://cancelacfd.sat.gob.mx")]
    [Serializable]
    public class Folios
    {
        private string uUIDField;
        private string estatusUUIDField;

        public string UUID
        {
            get
            {
                return this.uUIDField;
            }
            set
            {
                this.uUIDField = value;
            }
        }

        public string EstatusUUID
        {
            get
            {
                return this.estatusUUIDField;
            }
            set
            {
                this.estatusUUIDField = value;
            }
        }
    }
}
