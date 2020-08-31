using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Cotorra.Schema.CFDI33Nom12
{
    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://cancelacfd.sat.gob.mx")]
    [XmlRoot(Namespace = "http://cancelacfd.sat.gob.mx")]
    [Serializable]
    public class Cancelacion
    {
        private CancelacionFolios[] foliosField;
        private SignatureType signatureField;
        private string rfcEmisorField;
        private DateTime fechaField;

        [XmlElement("Folios", Order = 0)]
        public CancelacionFolios[] Folios
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

        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 1)]
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
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://cancelacfd.sat.gob.mx")]
    [Serializable]
    public class CancelacionFolios
    {
        private string uUIDField;

        [XmlElement(Order = 0)]
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
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class SignatureType
    {
        private SignedInfoType signedInfoField;
        private byte[] signatureValueField;
        private KeyInfoType keyInfoField;
        private ObjectType[] objectField;
        private string idField;

        public SignedInfoType SignedInfo
        {
            get
            {
                return this.signedInfoField;
            }
            set
            {
                this.signedInfoField = value;
            }
        }

        [XmlElement(DataType = "base64Binary")]
        public byte[] SignatureValue
        {
            get
            {
                return this.signatureValueField;
            }
            set
            {
                this.signatureValueField = value;
            }
        }

        public KeyInfoType KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        [XmlElement("Object")]
        public ObjectType[] Object
        {
            get
            {
                return this.objectField;
            }
            set
            {
                this.objectField = value;
            }
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class SignatureMethodType
    {
        private string hMACOutputLengthField;
        private XmlNode[] anyField;
        private string algorithmField;

        [XmlElement(DataType = "integer")]
        public string HMACOutputLength
        {
            get
            {
                return this.hMACOutputLengthField;
            }
            set
            {
                this.hMACOutputLengthField = value;
            }
        }

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class CanonicalizationMethodType
    {
        private XmlNode[] anyField;
        private string algorithmField;

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class TransformType
    {
        private string xPathField;
        private string[] textField;
        private string algorithmField;

        public string XPath
        {
            get
            {
                return this.xPathField;
            }
            set
            {
                this.xPathField = value;
            }
        }

        [XmlText]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class ReferenceType
    {
        private TransformType[] transformsField;
        private DigestMethodType digestMethodField;
        private byte[] digestValueField;
        private string typeField;
        private string uRIField;
        private string idField;

        [XmlArrayItem("Transform", IsNullable = false)]
        public TransformType[] Transforms
        {
            get
            {
                return this.transformsField;
            }
            set
            {
                this.transformsField = value;
            }
        }

        public DigestMethodType DigestMethod
        {
            get
            {
                return this.digestMethodField;
            }
            set
            {
                this.digestMethodField = value;
            }
        }

        [XmlElement(DataType = "base64Binary")]
        public byte[] DigestValue
        {
            get
            {
                return this.digestValueField;
            }
            set
            {
                this.digestValueField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class SignedInfoType
    {
        private CanonicalizationMethodType canonicalizationMethodField;
        private SignatureMethodType signatureMethodField;
        private ReferenceType referenceField;
        private string idField;

        public CanonicalizationMethodType CanonicalizationMethod
        {
            get
            {
                return this.canonicalizationMethodField;
            }
            set
            {
                this.canonicalizationMethodField = value;
            }
        }

        public SignatureMethodType SignatureMethod
        {
            get
            {
                return this.signatureMethodField;
            }
            set
            {
                this.signatureMethodField = value;
            }
        }

        public ReferenceType Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class RSAKeyValueType
    {
        private byte[] modulusField;
        private byte[] exponentField;

        [XmlElement(DataType = "base64Binary")]
        public byte[] Modulus
        {
            get
            {
                return this.modulusField;
            }
            set
            {
                this.modulusField = value;
            }
        }

        [XmlElement(DataType = "base64Binary")]
        public byte[] Exponent
        {
            get
            {
                return this.exponentField;
            }
            set
            {
                this.exponentField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class X509IssuerSerialType
    {
        private string x509IssuerNameField;
        private string x509SerialNumberField;

        public string X509IssuerName
        {
            get
            {
                return this.x509IssuerNameField;
            }
            set
            {
                this.x509IssuerNameField = value;
            }
        }

        [XmlElement(DataType = "integer")]
        public string X509SerialNumber
        {
            get
            {
                return this.x509SerialNumberField;
            }
            set
            {
                this.x509SerialNumberField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class X509DataType
    {
        private X509IssuerSerialType x509IssuerSerialField;
        private byte[] x509CertificateField;

        public X509IssuerSerialType X509IssuerSerial
        {
            get
            {
                return this.x509IssuerSerialField;
            }
            set
            {
                this.x509IssuerSerialField = value;
            }
        }

        [XmlElement(DataType = "base64Binary")]
        public byte[] X509Certificate
        {
            get
            {
                return this.x509CertificateField;
            }
            set
            {
                this.x509CertificateField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class KeyInfoType
    {
        private X509DataType x509DataField;
        private string keyNameField;
        private KeyValueType keyValueField;
        private string[] textField;
        private string idField;

        public X509DataType X509Data
        {
            get
            {
                return this.x509DataField;
            }
            set
            {
                this.x509DataField = value;
            }
        }

        public string KeyName
        {
            get
            {
                return this.keyNameField;
            }
            set
            {
                this.keyNameField = value;
            }
        }

        public KeyValueType KeyValue
        {
            get
            {
                return this.keyValueField;
            }
            set
            {
                this.keyValueField = value;
            }
        }

        [XmlText]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class KeyValueType
    {
        private RSAKeyValueType rSAKeyValueField;
        private string[] textField;

        public RSAKeyValueType RSAKeyValue
        {
            get
            {
                return this.rSAKeyValueField;
            }
            set
            {
                this.rSAKeyValueField = value;
            }
        }

        [XmlText]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class DigestMethodType
    {
        private XmlNode[] anyField;
        private string algorithmField;

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [Serializable]
    public class ObjectType
    {
        private XmlNode[] anyField;
        private string encodingField;
        private string mimeTypeField;
        private string idField;

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        [XmlAttribute(DataType = "anyURI")]
        public string Encoding
        {
            get
            {
                return this.encodingField;
            }
            set
            {
                this.encodingField = value;
            }
        }

        [XmlAttribute]
        public string MimeType
        {
            get
            {
                return this.mimeTypeField;
            }
            set
            {
                this.mimeTypeField = value;
            }
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
}
