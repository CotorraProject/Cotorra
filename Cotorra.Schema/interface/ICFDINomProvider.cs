using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public interface ICFDINomProvider
    {
        public string Certificado { get; set; }

        public string Sello { get; set; }

        public string NoCertificado { get; set; }
    }
}
