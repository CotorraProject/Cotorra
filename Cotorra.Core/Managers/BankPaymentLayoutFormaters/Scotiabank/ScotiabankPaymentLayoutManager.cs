using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Cotorra.Core.Managers;

namespace Cotorra.Core
{
    public class ScotiabankPaymentLayoutManager : IBankPaymentLayoutManager
    {
        private const string TypeRegistryFirstHeader = "EEHA";
        private const string TypeRegistrySecondHeader = "EEHB";
        private const string TypeRegistryDetail = "EEDA";
        private const string TypeRegistryFirstFooter = "EETB";
        private const string TypeRegistrySecondFooter = "EETA";
        private const short MaxLengthLine = 370;
        private const string AccountPaymentMethod = "04";

        private int detailCount;
        private OverdraftManager overdraftManager;

        public ScotiabankPaymentLayoutManager()
        {
            overdraftManager = new OverdraftManager();
        }

        public string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();
            var scotianbankAdditionalInformation = (ScotiabankAdditionalInformation) additionalInformation;

            paymentBankFormat.AppendLine(GenerateFirstHeaderRegistry(scotianbankAdditionalInformation));
            paymentBankFormat.AppendLine(GenerateSecondHeaderRegistry(scotianbankAdditionalInformation));
            paymentBankFormat.Append(GenerateDetailPaymentEmployees(overdrafts, scotianbankAdditionalInformation));
            paymentBankFormat.AppendLine(GenerateFootersRegistry(overdrafts));

            return paymentBankFormat.ToString();
        }

        private string GenerateFirstHeaderRegistry(ScotiabankAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}"
                , TypeRegistryFirstHeader
                , additionalInformation.CustomerNumber.PadLeft(5, '0')
                , additionalInformation.FileNumberOfDay.PadLeft(2, '0')
                , "".PadRight(27, '0')
                );

            string firstHeader = paymentBankFormat.ToString();

            return firstHeader.PadRight(MaxLengthLine, ' ');
        }

        private string GenerateSecondHeaderRegistry(ScotiabankAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}"
                , TypeRegistrySecondHeader
                , "000000"
                , additionalInformation.ChargeAccount.PadLeft(11, '0')
                , additionalInformation.CompanyReference.PadLeft(10, '0')
                , "000"
                );

            string secondHeader = paymentBankFormat.ToString();

            return secondHeader.PadRight(MaxLengthLine, ' ');

        }

        private string GenerateDetailPaymentEmployees(List<Overdraft> overdrafts, ScotiabankAdditionalInformation additionalInformation)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            detailCount = 0;
            string paymentDate = string.Format("{0:yyyyMMdd}", additionalInformation.PaymentDate);

            foreach (Overdraft payment in overdrafts)
            {
                detailCount++;
                decimal amountPayment = overdraftManager.GetNetAmount(payment);
                paymentBankFormat.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}"
                    , TypeRegistryDetail
                    , AccountPaymentMethod
                    , "00"
                    , amountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(15, '0')
                    , paymentDate
                    , "01"
                    , detailCount.ToString().PadRight(20, ' ')
                    , "".PadRight(13, ' ')
                    , FormatEmployeeName(payment.Employee.Name, payment.Employee.FirstLastName, payment.Employee.SecondLastName)
                    , detailCount.ToString().PadLeft(16, '0')
                    , "".PadLeft(10, '0')
                    , GetNoNullString(payment.Employee.BankAccount).PadLeft(20, '0')
                    , "00000"
                    , "".PadRight(40, ' ')
                    , FormatBankAccountType(GetNoNullString(payment.Employee.BankAccount))
                    , " "
                    , "00000"
                    , "044"
                    , "044"
                    , "001"
                    , "NOMINA".PadRight(50, ' ')
                    , "".PadRight(60, ' ')
                    , "".PadRight(25, '0')
                    , "".PadRight(22, ' ')
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

        private string FormatBankAccountType(string bankAccount)
        {
            string BankAccount = "1";
            string DebitCard = "3";
            short maxLengthBankAccount = 11;

            return (bankAccount.Length > maxLengthBankAccount) ? DebitCard : BankAccount;
        }

        private string GenerateFootersRegistry(List<Overdraft> overdrafts)
        {
            StringBuilder paymentBankFormat = new StringBuilder();

            var totalAmountPayment = overdraftManager.GetNetAmount(overdrafts);

            paymentBankFormat.AppendFormat("{0}{1}{2}"
                , detailCount.ToString().PadLeft(7, '0')
                , totalAmountPayment.ToString("#.00").Replace(".", string.Empty).PadLeft(17, '0')
                , "".PadRight(219, '0')
                );

            string commonFooter = paymentBankFormat.ToString().PadRight(MaxLengthLine - 4, ' ');

            return TypeRegistryFirstFooter + commonFooter
                   + Environment.NewLine
                   + TypeRegistrySecondFooter + commonFooter;
        }

        private string GetNoNullString(string checkString)
        {
            return string.IsNullOrEmpty(checkString) ? "" : checkString;
        }

    }
}
