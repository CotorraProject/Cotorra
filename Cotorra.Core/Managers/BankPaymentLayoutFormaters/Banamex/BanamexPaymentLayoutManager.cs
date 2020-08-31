using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Cotorra.Core.Managers;

namespace Cotorra.Core
{
    public class BanamexPaymentLayoutManager : IBankPaymentLayoutManager
    {
        private const string BankId = "0000";
        private OverdraftManager overdraftManager;
        private string totalPaymentOverdrafts;
        private int detailCount;

        public BanamexPaymentLayoutManager()
        {
            overdraftManager = new OverdraftManager();
        }

        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            var banamexAdditionalInformation = (BanamexAdditionalInformation)additionalInformation;
            var total = overdraftManager.GetNetAmount(overdrafts);
            totalPaymentOverdrafts = total.ToString("#.00").Replace(".", string.Empty).PadLeft(18, '0');

            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendLine(GenerateFirstHeaderRegistry(banamexAdditionalInformation));
            paymentBankFormat.AppendLine(GenerateSecondHeaderRegistry(banamexAdditionalInformation));
            paymentBankFormat.Append(GenerateDetailPaymentEmployees(overdrafts, banamexAdditionalInformation));
            paymentBankFormat.AppendLine(GenerateFooterRegistry());

            return paymentBankFormat.ToString();
        }

        private string GenerateFirstHeaderRegistry(BanamexAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}"
                , "1"
                , additionalInformation.CustomerNumber.PadLeft(12, '0')
                , string.Format("{0:ddMMyy}", additionalInformation.PaymentDate)
                , additionalInformation.FileNumberOfDay.PadLeft(4, '0')
                , additionalInformation.CompanyName.PadRight(36, ' ')
                , "DEPOSITOS POR NOMINA"
                , "05"
                , "".PadRight(40, ' ')
                , "B"
                , "00"
                );

            return paymentBankFormat.ToString();
        }

        private string GenerateSecondHeaderRegistry(BanamexAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}"
                , "2"
                , "1"
                , "001"
                , totalPaymentOverdrafts
                , "01"
                , additionalInformation.BranchOfficeNumber.PadLeft(4, '0')
                , additionalInformation.ChargeAccount.PadLeft(20, '0')
                , "".PadRight(18, ' ')
                );

            return paymentBankFormat.ToString();
        }

        private string GenerateDetailPaymentEmployees(List<Overdraft> overdrafts, BanamexAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();
            detailCount = 0;

            foreach (Overdraft payment in overdrafts)
            {
                detailCount++;
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}"
                    , "3"
                    , "0"
                    , "001"
                    , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(18, '0')
                    , "03"
                    , GetNoNullString(payment.Employee.BankBranchNumber).PadLeft(4, '0')
                    , GetNoNullString(payment.Employee.BankAccount).PadLeft(20, '0')
                    , payment.Employee.Code.PadRight(40, ' ')
                    , FormatEmployeeName(payment.Employee.Name, payment.Employee.FirstLastName, payment.Employee.SecondLastName)
                    , additionalInformation.StateID.PadRight(2, ' ')
                    , additionalInformation.CityID.PadLeft(4, '0')
                    , BankId
                    );

                paymentBankFormat.Append(Environment.NewLine);
            }

            return paymentBankFormat.ToString();
        }

        private string FormatEmployeeName(string name, string firstLastName, string secondLastName)
        {
            string fullName = string.Format("{0} {1} {2}",
                GetNoNullString(firstLastName),
                GetNoNullString(secondLastName),
                GetNoNullString(name));
            if (fullName.Length > 119)
            {
                return fullName.Substring(0, 119);
            }
            else
            {
                return fullName.PadRight(119, ' ');
            }
        }

        private string GenerateFooterRegistry()
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}"
                , "4"
                , "001"
                , detailCount.ToString().PadLeft(6, '0')
                , totalPaymentOverdrafts
                , "000001"
                , totalPaymentOverdrafts
                );

            return paymentBankFormat.ToString();
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }
    }
}
