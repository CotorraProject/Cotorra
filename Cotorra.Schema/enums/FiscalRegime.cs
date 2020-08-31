using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Regimen fiscal
    /// </summary>
    public enum FiscalRegime
    {
        /// <summary>
        /// General de Ley Personas Morales
        /// </summary>
        GeneralLawBusiness = 601,

        /// <summary>
        /// 603 = Personas Morales con Fines no Lucrativos
        /// </summary>
        NonProfitBusiness = 603,

        /// <summary>
        /// 605 = Sueldos y Salarios e Ingresos Asimilados a Salarios
        /// </summary>
        SalaryAndIncomeAssimilatedToWages = 605,

        /// <summary>
        /// 606 = Arrendamiento
        /// </summary>
        Lease = 606,

        /// <summary>
        /// 608 = Demás ingresos
        /// </summary>
        OtherIncome = 608,

        /// <summary>
        /// 610 = Residentes en el Extranjero sin Establecimiento Permanente en México
        /// </summary>
        ResidentsAbroadWithoutPermanentEstablishmentInMexico = 610,

        /// <summary>
        /// 611 = Ingresos por Dividendos (socios y accionistas)
        /// </summary>
        DividendIncomeByPartnersAndShareholders = 611,

        /// <summary>
        /// 612 = Personas Físicas con Actividades Empresariales y Profesionales
        /// </summary>
        IndividualsWithBusinessAndProfessionalActivities = 612,

        /// <summary>
        /// 614 = Ingresos por intereses
        /// </summary>
        InterestIncome = 614,

        /// <summary>
        /// 616 = Sin obligaciones fiscales
        /// </summary>
        NoTaxObligations = 616,

        /// <summary>
        /// 620 = Sociedades Cooperativas de Producción que optan por diferir sus ingresos
        /// </summary>
        CooperativeProductionSocietiesThatChooseToDeferTheirIncome = 620,

        /// <summary>
        /// 621 = Incorporación Fiscal   
        /// </summary>
        FiscalIncorporationPerson = 621,

        /// <summary>
        /// 622 = Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras 
        /// </summary>
        AgriculturalLivestockSilviculturalAndFishingActivities = 622,

        /// <summary>
        /// 623 = Opcional para Grupos de Sociedades
        /// </summary>
        OptionalForCompanyGroups = 623,

        /// <summary>
        /// 624 = Coordinados
        /// </summary>
        Coordinates = 624,

        /// <summary>
        /// 628 = Hidrocarburos
        /// </summary>
        Hydrocarbons = 628,

        /// <summary>
        /// 607 = Régimen de Enajenación o Adquisición de Bienes
        /// </summary>
        AlienationOrAcquisitionOfAssetsRegime = 607,

        /// <summary>
        /// 629 = De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales
        /// </summary>
        PreferentialTaxRegimesAndMultinationalCompanies = 629,

        /// <summary>
        /// 630 = Enajenación de acciones en bolsa de valores
        /// </summary>
        DisposalOfSharesOnTheStockExchange = 630,

        /// <summary>
        /// 615 = Régimen de los ingresos por obtención de premios
        /// </summary>
        RevenueSystemForObtainingPrizes = 615,

    }
}
