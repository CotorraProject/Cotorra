using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class PeriodTypesController : Controller
    {
        private readonly Client<PeriodType> client;
        private readonly Client<PeriodDetail> clientPeriodDetail;
        private readonly IClient<Period> clientPeriod;
        private readonly CalculationClient calculationClient;


        public PeriodTypesController()
        {
            SessionModel.Initialize();
            client = new Client<PeriodType>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
            clientPeriodDetail = new Client<PeriodDetail>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
            calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());

            clientPeriod  = new Client<Period>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        { 
            var periodDetails = (await clientPeriodDetail.FindAsync(p =>
                    p.InstanceID == SessionModel.InstanceID &&
                    (p.PeriodStatus == PeriodStatus.Calculating || p.PeriodStatus == PeriodStatus.Authorized),
                SessionModel.CompanyID, new string[] { "Period" })).OrderByDescending(x => x.Number);       

            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            var periodTypes = from pt in result
                              orderby pt.ExtraordinaryPeriod, pt.PeriodTotalDays
                              select new
                              {
                                  pt.ID,
                                  pt.Name,
                                  pt.ExtraordinaryPeriod,
                                  pt.PeriodTotalDays,
                                  pt.PaymentDays,
                                  pt.MonthCalendarFixed,
                                  pt.FortnightPaymentDays,
                                  PaymentPeriodicity = Convert.ToInt32(pt.PaymentPeriodicity).ToString().PadLeft(2, '0'),
                                  IsEnabled = periodDetails.Any(p => p.Period.PeriodTypeID == pt.ID),
                                  DetailInitialDate = periodDetails.FirstOrDefault(x=> x.Period.PeriodTypeID == pt.ID) != null  ? periodDetails.FirstOrDefault(x => x.Period.PeriodTypeID == pt.ID).InitialDate : DateTime.MinValue,
                                  DetailFinalDate = periodDetails.FirstOrDefault(x => x.Period.PeriodTypeID == pt.ID) != null ? periodDetails.FirstOrDefault(x => x.Period.PeriodTypeID == pt.ID).FinalDate : DateTime.MinValue,
                                  pt.HolidayPremiumPaymentType
                              };

            return Json(periodTypes);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(PeriodTypeDTO data)
        {
            var lstPeriodType = new List<PeriodType>();
            var seventhDays = 0;

            if (data.SeventhDayPosition != (int)WeeklySeventhDay.None)
            {
                seventhDays = 1;
            }
            var seventDayPosition = ((int)data.SeventhDayPosition).ToString();

            lstPeriodType.Add(new PeriodType()
            {
                ID = data.ID.HasValue ? data.ID.Value : Guid.NewGuid(),
                Description = data.Name,
                Name = data.Name,
                ExtraordinaryPeriod = data.ExtraordinaryPeriod,
                PaymentDays = data.PaymentDays,
                PeriodTotalDays = data.PeriodTotalDays,
                MonthCalendarFixed = data.MonthCalendarFixed,
                FortnightPaymentDays = (AdjustmentPay_16Days_Febrary)data.FortnightPaymentDays,
                PaymentPeriodicity = (PaymentPeriodicity)data.PaymentPeriodicity,
                SeventhDayPosition = seventDayPosition,
                SeventhDays = seventhDays,
                //Common
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
                HolidayPremiumPaymentType = (HolidayPaymentConfiguration) data.HolidayPremiumPaymentType,
            });

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstPeriodType, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstPeriodType, SessionModel.IdentityID);
                await UpdateFollowingDetails(data.ID.Value, seventhDays, seventDayPosition, data.PaymentDays);
            }

            return Json(lstPeriodType.FirstOrDefault().ID);
        }

        private async Task UpdateFollowingDetails(Guid periodTypeID, int seventhDays, string seventhDayPosition, decimal paymentDays)
        {
            var periodDetails = await clientPeriodDetail.FindAsync(p =>
                    p.InstanceID == SessionModel.InstanceID && p.Period.PeriodTypeID == periodTypeID &&
                   (p.PeriodStatus == PeriodStatus.Open || p.PeriodStatus == PeriodStatus.Calculating),
                SessionModel.CompanyID, new string[] { "Period" });

            var calculatingPeriodDetail = periodDetails.FirstOrDefault(x => x.PeriodStatus == PeriodStatus.Calculating);

            periodDetails.AsParallel().ForAll(x =>
            {
                x.SeventhDays = seventhDays;
                x.SeventhDayPosition = seventhDayPosition;
                x.PaymentDays = paymentDays;
                x.Period = null;
            });
            await clientPeriodDetail.UpdateAsync(periodDetails, SessionModel.CompanyID);
            await Recalculate(calculatingPeriodDetail.PeriodID);

        }

        private async Task Recalculate(Guid periodID)
        {
            await calculationClient.CalculationFireAndForgetByPeriodIdsAsync(new CalculationFireAndForgetByPeriodParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                PeriodIds = new List<Guid>() { periodID },
                UserID = SessionModel.IdentityID
            });
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class PeriodTypeDTO
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }
            public Boolean ExtraordinaryPeriod { get; set; }
            public Int32 PeriodTotalDays { get; set; }
            public Decimal PaymentDays { get; set; }
            public Int32 FortnightPaymentDays { get; set; }
            public Int32 PaymentPeriodicity { get; set; }
            public Boolean MonthCalendarFixed { get; set; }
            public int SeventhDayPosition { get; set; }
            public int HolidayPremiumPaymentType { get; set; }

        }
    }
}
