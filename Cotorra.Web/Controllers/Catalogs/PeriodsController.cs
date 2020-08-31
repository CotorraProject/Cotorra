using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Security.Permissions;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class PeriodsController : Controller
    {
        private readonly Client<PeriodType> clientPT;
        private readonly Client<PeriodDetail> clientPD;
        private readonly Client<Period> clientPeriod;
        private readonly CalculationClient calculationClient;
        public PeriodsController()
        {
            SessionModel.Initialize();
            var adapterFromConfig = ClientConfiguration.GetAdapterFromConfig();
            clientPT = new Client<PeriodType>(SessionModel.AuthorizationHeader, clientadapter: adapterFromConfig);
            clientPD = new Client<PeriodDetail>(SessionModel.AuthorizationHeader, clientadapter: adapterFromConfig);
            clientPeriod = new Client<Period>(SessionModel.AuthorizationHeader, clientadapter: adapterFromConfig);
            calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, clientadapter: adapterFromConfig);
        }


        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var findResult = await clientPT.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID && x.ExtraordinaryPeriod == false,
                SessionModel.CompanyID, new String[] { "Periods", "Periods.PeriodDetails" });

            var result = new List<Object>();
            var periodTypes = findResult.OrderBy(x => x.ExtraordinaryPeriod).ToList();

            for (int i = 0; i < periodTypes.Count(); i++)
            {
                var pt = periodTypes[i];

                //Add root level
                result.Add(new
                {
                    //Tree properties
                    id = pt.ID,
                    parentId = (Object)null,

                    //General
                    ID = pt.ID,
                    ParentID = (Object)null,
                    Name = pt.Name,
                    Type = "PeriodType",

                    //PeriodTypeInfo
                    HasChildren = pt.Periods.Count() > 0,
                    pt.ExtraordinaryPeriod,
                    pt.PeriodTotalDays,
                    pt.PaymentDays,
                    pt.MonthCalendarFixed,
                    pt.FortnightPaymentDays,
                    pt.SeventhDayPosition,
                    PaymentPeriodicity = Convert.ToInt32(pt.PaymentPeriodicity).ToString().PadLeft(2, '0'),
                    pt.HolidayPremiumPaymentType

                });

                //Add childs
                if (pt.Periods.Count() > 0)
                {
                    var periodsResult =
                        (from p in pt.Periods
                         orderby p.FiscalYear
                         where p.PeriodTypeID == pt.ID
                         select new
                         {
                             //Tree properties
                             id = p.ID,
                             parentId = p.PeriodTypeID,
                             p.IsActualFiscalYear,
                             //General
                             ID = p.ID,
                             ParentID = p.PeriodTypeID,
                             Name = p.FiscalYear,
                             Type = "FiscalYear",
                             Year = p.FiscalYear
                         }).ToList();

                    result.AddRange(periodsResult);
                }
                else
                {
                    result.Add(new
                    {
                        //Tree properties
                        id = Guid.NewGuid(),
                        parentId = pt.ID,

                        //PeriodTypes
                        ID = Guid.NewGuid(),
                        ParentID = pt.ID,
                        Name = "Sin ejercicios",
                        Type = "NoPeriodTypes",
                    });
                }
            }

            return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetDetails(Guid periodID)
        {
            var periodDetails = await clientPD.FindAsync(x =>
                x.Active == true && x.PeriodID == periodID && x.InstanceID == SessionModel.InstanceID,
                SessionModel.CompanyID, new String[] { "Period" });

            if (periodDetails.Count() == 0)
            {
                return Json(new List<Object>());
            }

            var result = (from pd in periodDetails
                          orderby pd.Number
                          select new
                          {
                              pd.ID,

                              FiscalYear = pd.FinalDate.Year,
                              pd.Number,

                              pd.InitialDate,
                              pd.FinalDate,

                              Month = pd.FinalDate.Month,
                              pd.PaymentDays,

                              pd.PeriodMonth,
                              pd.PeriodBimonthlyIMSS,
                              pd.PeriodFiscalYear,

                              pd.PeriodStatus
                          }).ToList();

            return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetActualDetail(Guid periodTypeID)
        {
            var periodDetailss = (await clientPD.FindAsync(x =>
                   x.Period.PeriodTypeID == periodTypeID &&
                    x.Active == true &&
                   x.InstanceID == SessionModel.InstanceID &&
                   (x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized),
                   SessionModel.CompanyID, new String[] { "Period" })).OrderByDescending(x => x.Number);


            var periodDetails = periodDetailss.First();
            var result = new
            {
                periodDetails.ID,

                FiscalYear = periodDetails.FinalDate.Year,
                periodDetails.Number,

                periodDetails.InitialDate,
                periodDetails.FinalDate,

                Month = periodDetails.FinalDate.Month,
                periodDetails.PaymentDays,

                periodDetails.PeriodMonth,
                periodDetails.PeriodBimonthlyIMSS,
                periodDetails.PeriodFiscalYear,

            };

            return Json(result);
        }


        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(PeriodDTO data)
        {

            //Get the period type base template
            var periodType = (await clientPT.FindAsync(x => x.ID == data.PeriodTypeId,
               SessionModel.CompanyID, new String[] { })).FirstOrDefault();

            //First update data of template period type
            periodType.PaymentDays = data.PaymentDays;
            periodType.SeventhDays = (data.SeventhDayPosition != (int)WeeklySeventhDay.None ? 1 : 0);
            periodType.SeventhDayPosition = data.SeventhDayPosition.ToString();
            periodType.FortnightPaymentDays = data.FortnightPaymentDays;

            if (!data.IsEditing)
            {
                //Enable the first fiscal year
                var resultID = Guid.NewGuid();
                var year = data.Year;

                var initialDate = data.InitialDate;
                var finalDate = new DateTime(year, 12, 31);

                //create period (fiscal year)
                var Periods = new List<Period>
                {
                    new Period()
                    {
                        ID = resultID,
                        Active = true,
                        company = SessionModel.CompanyID,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = SessionModel.InstanceID,
                        Description = $"Periodo del Ejercicio {year}",
                        CreationDate = DateTime.Now,
                        Name = $"{year}",
                        InitialDate = initialDate,
                        FinalDate = finalDate,
                        FiscalYear = data.Year,
                        IsActualFiscalYear = year == DateTime.Now.Year,
                        IsFiscalYearClosed = false,
                        PeriodTypeID = periodType.ID,
                        ExtraordinaryPeriod = periodType.ExtraordinaryPeriod,
                        FortnightPaymentDays = periodType.FortnightPaymentDays,
                        MonthCalendarFixed = periodType.MonthCalendarFixed,
                        PaymentDayPosition = periodType.PaymentDayPosition,
                        PaymentDays = periodType.PaymentDays,
                        PaymentPeriodicity = periodType.PaymentPeriodicity,
                        PeriodTotalDays = periodType.PeriodTotalDays,
                        StatusID = 1,
                        SeventhDayPosition = periodType.SeventhDayPosition,
                        SeventhDays = periodType.SeventhDays
                    }
                };

                await clientPeriod.CreateAsync(Periods, SessionModel.CompanyID);

                var newPeriodDetails = await clientPD.FindAsync(x => x.PeriodID == resultID, SessionModel.CompanyID, new String[] { });
                for (int i = 0; i < newPeriodDetails.Count(); i++)
                {
                    var pd = newPeriodDetails[i];
                    if (pd.Number < data.CurrentPeriod) { pd.PeriodStatus = PeriodStatus.NA; }
                    else if (pd.Number == data.CurrentPeriod) { pd.PeriodStatus = PeriodStatus.Calculating; }
                    else if (pd.Number > data.CurrentPeriod) { pd.PeriodStatus = PeriodStatus.Open; }

                }

                await clientPD.UpdateAsync(newPeriodDetails, SessionModel.CompanyID);
            }

            //Update period type
            await clientPT.UpdateAsync(new List<PeriodType>() { periodType }, SessionModel.CompanyID);

            //Recalculate
            await RecalculateAsync(periodType);

            return Json("OK");
        }

        private async Task RecalculateAsync(PeriodType periodType)
        {
            //Get periods of periodType to Recalculte
            var periods = await clientPeriod.FindAsync(p =>
                p.PeriodTypeID == periodType.ID && p.InstanceID == periodType.InstanceID,
                periodType.company);

            await calculationClient.CalculationFireAndForgetByPeriodIdsAsync(new CalculationFireAndForgetByPeriodParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                PeriodIds = periods.Select(p => p.ID).ToList(),
                UserID = SessionModel.IdentityID
            });
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await Task.Run(() =>
            {

            });

            return Json("OK");
        }
    }

    public class PeriodDTO
    {
        public Guid PeriodTypeId { get; set; }
        public Int32 Year { get; set; }
        public DateTime InitialDate { get; set; }
        public Decimal PaymentDays { get; set; }
        public Int32 SeventhDayPosition { get; set; }
        public AdjustmentPay_16Days_Febrary FortnightPaymentDays { get; set; }
        public Boolean IsEditing { get; set; }
        public Int32 CurrentPeriod { get; set; }

    }
}
