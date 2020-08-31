using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class ConceptsController : Controller
    {
        private readonly Client<ConceptPayment> client;
        private IMemoryCache _cache;


        public ConceptsController(IMemoryCache memoryCache)
        {
            SessionModel.Initialize();
            client = new Client<ConceptPayment>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
            _cache = memoryCache;
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(String type)
        {
            ConceptType conceptType = ConceptType.SalaryPayment;
            if (type == "P") { conceptType = ConceptType.SalaryPayment; }
            else if (type == "D") { conceptType = ConceptType.DeductionPayment; }
            else if (type == "L") { conceptType = ConceptType.LiabilityPayment; }

            Expression<Func<ConceptPayment, bool>> expression;
            if (String.IsNullOrEmpty(type)) { expression = (x => x.Active && x.InstanceID == SessionModel.InstanceID); }
            else { expression = (x => x.Active && x.InstanceID == SessionModel.InstanceID && x.ConceptType == conceptType); }

            var result = await client.FindAsync(expression,
                SessionModel.CompanyID, new string[] { 
                /*"ConceptPaymentRelationship",*/
                /*"ConceptPaymentRelationship.AccumulatedType"*/ });
            var concepts = from r in result.AsParallel()
                           orderby r.Code
                           select new
                           {
                               //General
                               r.ID,
                               r.Code,
                               r.Name,
                               r.SATGroupCode,
                               r.GlobalAutomatic,
                               r.AutomaticDismissal,
                               r.Kind,
                               r.Print,
                               r.ConceptType,

                               //Total Amount
                               Label = String.IsNullOrEmpty(r.Label) ? "" : r.Label,
                               r.Formula,

                               //Amount 1
                               Label1 = String.IsNullOrEmpty(r.Label1) ? "" : r.Label1,
                               r.Formula1,

                               //Amount 1
                               Label2 = String.IsNullOrEmpty(r.Label2) ? "" : r.Label2,
                               r.Formula2,

                               //Amount 3
                               Label3 = String.IsNullOrEmpty(r.Label3) ? "" : r.Label3,
                               r.Formula3,

                               //Amount 4
                               Label4 = String.IsNullOrEmpty(r.Label4) ? "" : r.Label4,
                               r.Formula4,

                               //ConceptPaymentRelationship = (from cpr in r.ConceptPaymentRelationship
                               //                              select new
                               //                              {
                               //                                  ID = cpr.AccumulatedTypeID,
                               //                                  Name = cpr.AccumulatedType.Name,
                               //                                  RelationType = cpr.ConceptPaymentRelationshipType,
                               //                                  TypeOfAccumulated = cpr.AccumulatedType.TypeOfAccumulated,
                               //                                  ConceptPaymentType = cpr.ConceptPaymentType
                               //                              }).ToList()
                           };

            return Json(concepts.OrderBy(p => p.Code));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Concept concept)
        {
            //siempre y cuando sea nuevo registro
            if (concept.Code <= 500 && !concept.ID.HasValue)
            {
                throw new CotorraException(106, "106", "No es posible crear conceptos menores a 500, están reservados por el sistema. ", null);
            }

            ConceptType ct;
            switch (concept.ConceptType)
            {
                case "P": ct = ConceptType.SalaryPayment; break;
                case "D": ct = ConceptType.DeductionPayment; break;
                case "L": ct = ConceptType.LiabilityPayment; break;
                default: ct = ConceptType.SalaryPayment; break;
            }

            var data = new ConceptPayment
            {
                //General
                Code = concept.Code,
                Name = concept.Name,
                SATGroupCode = concept.SATGroupCode == null ? String.Empty : concept.SATGroupCode,
                GlobalAutomatic = concept.GlobalAutomatic,
                AutomaticDismissal = concept.AutomaticDismissal,
                Kind = concept.Kind,
                Print = concept.Print,
                Description = concept.Name,
                ConceptType = ct,

                //Total
                Label = concept.Label,
                Formula = concept.Formula,

                //Amount1
                Label1 = concept.Label1,
                Formula1 = concept.Formula1,

                //Amount2
                Label2 = concept.Label2,
                Formula2 = concept.Formula2,

                //Amount3
                Label3 = concept.Label3,
                Formula3 = concept.Formula3,

                //Amount4
                Label4 = concept.Label4,
                Formula4 = concept.Formula4,

                //Common
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
            };

            if (concept.ConceptType == "P")
            {
                data.ConceptType = ConceptType.SalaryPayment;
            }
            else if (concept.ConceptType == "D")
            {
                data.ConceptType = ConceptType.DeductionPayment;
            }
            else if (concept.ConceptType == "L")
            {
                data.ConceptType = ConceptType.LiabilityPayment;
            }

            if (!concept.ID.HasValue)
            {
                //Save
                data.ID = Guid.NewGuid();
                await client.CreateAsync(new List<ConceptPayment> { data }, SessionModel.IdentityID);
            }
            else
            {
                //Update
                data.ID = concept.ID.Value;
                await client.UpdateAsync(new List<ConceptPayment> { data }, SessionModel.IdentityID);
            }

            return Json(data.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id, String ConceptType)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.IdentityID);
            return Json("OK");
        }

        #region private methods

        #endregion private methods
    }

    public class Concept
    {
        public Guid? ID { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public bool GlobalAutomatic { get; set; }
        public bool Kind { get; set; }
        public bool AutomaticDismissal { get; set; }
        public bool Print { get; set; }
        public string SATGroupCode { get; set; }
        public string ConceptType { get; set; }

        //Labels
        public String Label { get; set; }
        public String Label1 { get; set; }
        public String Label2 { get; set; }
        public String Label3 { get; set; }
        public String Label4 { get; set; }

        //Formulas
        public String Formula { get; set; }
        public String Formula1 { get; set; }
        public String Formula2 { get; set; }
        public String Formula3 { get; set; }
        public String Formula4 { get; set; }

        //Accumulateds
        public List<AccumulatedItem> TotalAmountFiscalAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmountOtherAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount1FiscalAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount1OtherAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount2FiscalAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount2OtherAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount3FiscalAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount3OtherAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount4FiscalAccumulated { set; get; }
        public List<AccumulatedItem> TotalAmount4OtherAccumulated { set; get; }
    }

    public class AccumulatedItem
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String TypeOfAccumulated { get; set; }
    }
}