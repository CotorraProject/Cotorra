using System;
using System.Collections.Generic;
using System.Text;
using Cotorra.Schema;
using Cotorra.Core.Managers;

namespace Cotorra.Core
{ 
    public class BanortePaymentLayoutManager : IBankPaymentLayoutManager
    {
        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            var banorteAdditionalInformation = (BanorteAdditionalInformation)additionalInformation;
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendLine(GenerateHeaderRegistry(banorteAdditionalInformation));
            paymentBankFormat.Append(GenerateDetailPaymentEmployees(overdrafts, banorteAdditionalInformation));

            return paymentBankFormat.ToString();
        }

        private string GenerateHeaderRegistry(BanorteAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}"
                , string.Format("{0:yyyyMMdd}", additionalInformation.PaymentDate)
                , additionalInformation.ChargeAccount.PadLeft(9, '0')
                , FormatCompanyName(additionalInformation.CompanyName)
                , "#".PadLeft(5, ' ')
                );

            return paymentBankFormat.ToString();
        }

        private string GenerateDetailPaymentEmployees(List<Overdraft> overdrafts, BanorteAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            OverdraftManager overdraftManager = new OverdraftManager();

            foreach (Overdraft payment in overdrafts)
            {
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}"
                    , FormatEmployeeName(payment.Employee.Name, payment.Employee.FirstLastName, payment.Employee.SecondLastName)
                    , additionalInformation.SystemID.PadLeft(3, '0')
                    , GetNoNullString(payment.Employee.BankAccount).PadLeft(9, '0')
                    , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(14, '0')
                    , payment.Employee.Code.PadLeft(10, '0')
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

            if (fullName.Length > 26)
            {
                return fullName.Substring(0, 26);
            }
            else
            {
                return fullName.PadRight(26, ' ');
            }
        }

        private string FormatCompanyName(string name)
        {
            if (name.Length > 40)
            {
                return name.Substring(0, 40);
            }
            else
            {
                return name.PadRight(40, ' ');
            }
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }
    }
}
