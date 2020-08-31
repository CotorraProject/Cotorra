using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class FiscalRegimeInformation
    {
        [DataMember]
        public FiscalRegime FiscalRegime { get; set; }

        [DataMember]
        public bool IsPhysicalPerson { get; set; }

        [DataMember]
        public bool IsMoralPerson { get; set; }

        [DataMember]
        public String Description{ get; set; }
    }

    [DataContract]
    public class FiscalRegimeDetails
    {
        private readonly List<FiscalRegimeInformation> FiscalRegimeInformations;

        public FiscalRegimeDetails()
        {
            FiscalRegimeInformations = new List<FiscalRegimeInformation>();
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "General de ley personas morales", FiscalRegime = (FiscalRegime)601, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Personas morales con fines no lucrativos", FiscalRegime = (FiscalRegime)603, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Sueldos y salarios e ingresos asimilados a asalarios", FiscalRegime = (FiscalRegime)605, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Arrendamiento", FiscalRegime = (FiscalRegime)606, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Demás ingresos", FiscalRegime = (FiscalRegime)608, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Residentes en el extranjero sin establecimiento permanente en México", FiscalRegime = (FiscalRegime)610, IsPhysicalPerson = true, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Ingresos por dividendos (socios y accionistas)", FiscalRegime = (FiscalRegime)611, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Personas físicas con actividades empresariales y profesionales", FiscalRegime = (FiscalRegime)612, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Ingresos por intereses", FiscalRegime = (FiscalRegime)614, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Sin obligaciones fiscales", FiscalRegime = (FiscalRegime)616, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Sociedades cooperativas de producción que optan por diferir sus ingresos", FiscalRegime = (FiscalRegime)620, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Incorporación fiscal", FiscalRegime = (FiscalRegime)621, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Actividades agrícolas, ganaderas, silvícolas y pesqueras ", FiscalRegime = (FiscalRegime)622, IsPhysicalPerson = true, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Opcional para grupos de sociedades", FiscalRegime = (FiscalRegime)623, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Coordinados", FiscalRegime = (FiscalRegime)624, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Hidrocarburos", FiscalRegime = (FiscalRegime)628, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Régimen de enajenación o adquisición de bienes", FiscalRegime = (FiscalRegime)607, IsPhysicalPerson = false, IsMoralPerson = true });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "De los regímenes fiscales preferentes y de las empresas multinacionales", FiscalRegime = (FiscalRegime)629, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Enajenación de acciones en bolsa de valores", FiscalRegime = (FiscalRegime)630, IsPhysicalPerson = true, IsMoralPerson = false });
            FiscalRegimeInformations.Add(new FiscalRegimeInformation() { Description = "Régimen de los ingresos por obtención de premios", FiscalRegime = (FiscalRegime)615, IsPhysicalPerson = true, IsMoralPerson = false });
        }

        public List<FiscalRegimeInformation> GetFiscalRegimeInformation()
        {
            return FiscalRegimeInformations;
        }

        public FiscalRegimeInformation GetFiscalRegimeInformationByFiscalRegime(FiscalRegime fiscalRegime)
        {
            return FiscalRegimeInformations.FirstOrDefault(p => p.FiscalRegime == fiscalRegime);
        }
    }
}
