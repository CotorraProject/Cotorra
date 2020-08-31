using Cotorra.Core.Managers;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core
{ 
    public class BBVAPaymentLayoutManager : IBankPaymentLayoutManager
    {
        private const string BankAccountType = "99";
        private const string BankDestination = "001";

        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            int countPayment = 1;

            StringBuilder paymentBankFormat = new StringBuilder();
            OverdraftManager overdraftManager = new OverdraftManager();

            foreach (Overdraft payment in overdrafts)
            {
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}"
                    , countPayment.ToString().PadLeft(9, '0')
                    , GetNoNullString(payment.Employee.RFC).PadRight(16, ' ')
                    , BankAccountType
                    , GetNoNullString(payment.Employee.BankAccount).PadRight(20, ' ')
                    , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(15, '0')
                    , FormatEmployeeName(payment.Employee.FullName)
                    , BankDestination
                    , BankDestination);

                paymentBankFormat.Append(Environment.NewLine);

                countPayment++;
            }

            return paymentBankFormat.ToString();
        }

        private string FormatEmployeeName(string employeeName)
        {
            if (employeeName.Length > 40)
            {
                return employeeName.Substring(0, 40).PadRight(40, ' ');
            }
            else
            {
                return employeeName.PadRight(40, ' ');
            }
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }
    }
}
