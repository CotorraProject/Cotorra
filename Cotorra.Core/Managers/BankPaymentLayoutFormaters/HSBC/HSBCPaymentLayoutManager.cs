using Cotorra.Core.Managers;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core
{
    public class HSBCPaymentLayoutManager : IBankPaymentLayoutManager
    {
        private const string ReferencePayment = "Abono por nómina";

        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();
            OverdraftManager overdraftManager = new OverdraftManager();

            foreach(Overdraft payment in overdrafts)
            {
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}"
                        , GetNoNullString(payment.Employee.BankAccount).PadLeft(10, '0')
                        , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(13, '0')
                        , ReferencePayment.PadRight(30, ' ')
                        , FormatEmployeeName(payment.Employee.Name, payment.Employee.FirstLastName, payment.Employee.SecondLastName)
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
            if (fullName.Length > 40)
            {
                return fullName.Substring(0, 40);
            }
            else
            {
                return fullName.PadRight(40, ' ');
            }
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }
    }
}
