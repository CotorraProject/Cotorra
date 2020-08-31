using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Cotorra.Schema;
using Cotorra.Core;

namespace Cotorra.UnitTest
{
    public class BanortePaymentLayoutManagerUT
    {
        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "20200704998855667Regresivas1241                              #"
                               + Environment.NewLine
                               + "Rodriguez Sanchez Ivan    007071235489000000001968200000000007"
                               + Environment.NewLine;

            BanortePaymentLayoutManager banorteLayout = new BanortePaymentLayoutManager();

            BanorteAdditionalInformation additionalInformation = new BanorteAdditionalInformation
            {
                SystemID = "007",
                ChargeAccount = "998855667",
                PaymentDate = new DateTime(2020, 7, 4),
                CompanyName = "Regresivas1241",
            };

            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1968.2m,
                Value = 15,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = "71235489";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            employee.Code = "7";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banorteLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Should_Create_Layout_No_Employee_Bank_Account()
        {
            //Arrange
            string expected = "20200704998855667Regresivas1241                              #"
                               + Environment.NewLine
                               + "Rodriguez Sanchez Ivan    007000000000000000001968200000000007"
                               + Environment.NewLine;

            BanortePaymentLayoutManager banorteLayout = new BanortePaymentLayoutManager();

            BanorteAdditionalInformation additionalInformation = new BanorteAdditionalInformation
            {
                SystemID = "007",
                ChargeAccount = "998855667",
                PaymentDate = new DateTime(2020, 7, 4),
                CompanyName = "Regresivas1241",
            };

            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1968.2m,
                Value = 15,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = null;
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            employee.Code = "7";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banorteLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Should_Create_Layout_Long_Company_Name()
        {
            //Arrange
            string expected = "20200704998855667La empresa de plasticos y desechables de    #"
                               + Environment.NewLine
                               + "Rodriguez Sanchez Ivan    007071235489000000001968200000000007"
                               + Environment.NewLine;

            BanortePaymentLayoutManager banorteLayout = new BanortePaymentLayoutManager();

            BanorteAdditionalInformation additionalInformation = new BanorteAdditionalInformation
            {
                SystemID = "007",
                ChargeAccount = "998855667",
                PaymentDate = new DateTime(2020, 7, 4),
                CompanyName = "La empresa de plasticos y desechables de Mexico",
            };

            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1968.2m,
                Value = 15,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = "71235489";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            employee.Code = "7";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banorteLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overdraft()
        {
            //Arrange
            string expected = "20200704998855667Regresivas1241                              #"
                               + Environment.NewLine
                               + "Rodriguez Sanchez Ivan    007071235489000000001968200000000007"
                               + Environment.NewLine
                               + "Funco Salon Demian        007271234485000000004502000000000006"
                               + Environment.NewLine;

            BanortePaymentLayoutManager banorteLayout = new BanortePaymentLayoutManager();

            BanorteAdditionalInformation additionalInformation = new BanorteAdditionalInformation
            {
                SystemID = "007",
                ChargeAccount = "998855667",
                PaymentDate = new DateTime(2020, 7, 4),
                CompanyName = "Regresivas1241",
            };

            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1968.2m,
                Value = 15,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = "71235489";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            employee.Code = "7";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Employee 2
            overdraftDetails = new List<OverdraftDetail>();

            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 4000.0m,
                Value = 7,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Bono",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1155.22m,
                Value = 0,
            };

            overdraftDetails.Add(detail);


            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 134.81m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "ISR",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 518.41m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "271234485";
            employee.FirstLastName = "Funco";
            employee.SecondLastName = "Salon";
            employee.Name = "Demian";
            employee.Code = "006";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banorteLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }
    }
}
