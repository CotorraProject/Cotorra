using Cotorra.Core.Managers;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core
{
    public class SantanderPaymentLayoutManager : IBankPaymentLayoutManager
    {
        private const string TypeRegistryHeader = "1";
        private const string TypeRegistryDetail = "2";
        private const string TypeREgistryFooter = "3";
        private const int MaxLengthName = 30;
        private const int MaxLengthSecondLastName = 20;
        private int RowFileCount;
        private int DetailCount;
        private OverdraftManager overdraftManager;

        public SantanderPaymentLayoutManager()
        {
            overdraftManager = new OverdraftManager();
        }

        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            var santanderAdditionalInformation = (SantanderAdditionalInformation) additionalInformation;
            RowFileCount = 1;
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendLine(GenerateHeaderRegistry(santanderAdditionalInformation));
            paymentBankFormat.AppendLine(GenerateDetailPaymentEmployees(overdrafts));
            RowFileCount++;
            paymentBankFormat.AppendLine(GenerateFooterRegistry(overdrafts));

            return paymentBankFormat.ToString();
        }

        private string GenerateHeaderRegistry(SantanderAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}"
                , TypeRegistryHeader
                , RowFileCount.ToString().PadLeft(5, '0')
                , 'E'
                , string.Format("{0:MMddyyyy}", additionalInformation.GenerationDate)
                , FormatCompanyBankAccount(additionalInformation.CompanyBankAccount)
                , string.Format("{0:MMddyyyy}", additionalInformation.PaymentDate)
                );

            return paymentBankFormat.ToString();
        }

        private string GenerateDetailPaymentEmployees(List<Overdraft> overdrafts)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            DetailCount = 1;

            foreach (Overdraft payment in overdrafts)
            {
                RowFileCount++;
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}"
                    , TypeRegistryDetail
                    , RowFileCount.ToString().PadLeft(5, '0')
                    , FormatEmployeeCode(payment.Employee.Code)
                    , FormatEmployeeName(GetNoNullString(payment.Employee.FirstLastName).ToUpper(), MaxLengthName)
                    , FormatEmployeeName(GetNoNullString(payment.Employee.SecondLastName).ToUpper(), MaxLengthSecondLastName)
                    , FormatEmployeeName(GetNoNullString(payment.Employee.Name).ToUpper(), MaxLengthName)
                    , GetNoNullString(payment.Employee.BankAccount).PadRight(16, ' ')
                    , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(18, '0')
                    );

                if (DetailCount < overdrafts.Count)
                {
                    paymentBankFormat.Append(Environment.NewLine);
                }

                DetailCount++;
            }

            return paymentBankFormat.ToString();

        }

        private string GenerateFooterRegistry(List<Overdraft> overdrafts)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            var totalAmountPayment = overdraftManager.GetNetAmount(overdrafts);

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}"
                , TypeREgistryFooter
                , RowFileCount.ToString().PadLeft(5, '0')
                , overdrafts.Count.ToString().PadLeft(5, '0')
                , totalAmountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(18, '0')
                );

            return paymentBankFormat.ToString();
        }

        private string FormatCompanyBankAccount(string account)
        {
            short MaxLength = 16;

            account = string.IsNullOrEmpty(account) ? "" : account;
            account = (account.Length > MaxLength) ? account.Substring(0, MaxLength) : account.PadRight(MaxLength, ' ');

            return account;
        }

        private string FormatEmployeeName(string name, int maxLength)
        {
            name = (name.Length > maxLength) ? name.Substring(0, maxLength) : name.PadRight(maxLength, ' ');

            return name;
        }

        private string FormatEmployeeCode(string code)
        {
            short MaxLength = 7;

            code = (code.Length > MaxLength) ? code.Substring(code.Length - MaxLength, MaxLength) : code.PadLeft(7, '0');

            return code;
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }
    }
}
