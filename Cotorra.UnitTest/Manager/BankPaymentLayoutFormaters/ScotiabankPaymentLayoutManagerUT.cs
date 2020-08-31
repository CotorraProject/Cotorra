using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class ScotiabankPaymentLayoutManagerUT
    {

        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "EEHA9957601000000000000000000000000000                                                                                                                                                                                                                                                                                                                                            "
                               + Environment.NewLine
                               + "EEHB000000032003348041234567899000                                                                                                                                                                                                                                                                                                                                                "
                               + Environment.NewLine
                               + "EEDA040000000000009232020200704011                                ESCUDERO IGNACIO JULIA                  000000000000000100000000000000000000320077000000000                                        1 00000044044001NOMINA                                                                                                        0000000000000000000000000                      "
                               + Environment.NewLine
                               + "EETB000000100000000000092320000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine
                               + "EETA000000100000000000092320000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine;

            ScotiabankPaymentLayoutManager scotiabankLayout = new ScotiabankPaymentLayoutManager();

            ScotiabankAdditionalInformation additionalInformation = new ScotiabankAdditionalInformation
            {
                CustomerNumber = "99576",
                ChargeAccount = "3200334804",
                FileNumberOfDay = "1",
                CompanyReference = "1234567899",
                PaymentDate = new DateTime(2020, 7, 4),
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
                Amount = 923.2m,
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
            employee.BankAccount = "03200770000";
            employee.FirstLastName = "ESCUDERO";
            employee.SecondLastName = "IGNACIO";
            employee.Name = "JULIA";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = scotiabankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Should_Create_Layout_No_Second_Last_Name()
        {
            //Arrange
            string expected = "EEHA9957601000000000000000000000000000                                                                                                                                                                                                                                                                                                                                            "
                               + Environment.NewLine
                               + "EEHB000000032003348041234567899000                                                                                                                                                                                                                                                                                                                                                "
                               + Environment.NewLine
                               + "EEDA040000000000009232020200704011                                ESCUDERO  JULIA                         000000000000000100000000000000000000320077000000000                                        1 00000044044001NOMINA                                                                                                        0000000000000000000000000                      "
                               + Environment.NewLine
                               + "EETB000000100000000000092320000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine
                               + "EETA000000100000000000092320000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine;

            ScotiabankPaymentLayoutManager scotiabankLayout = new ScotiabankPaymentLayoutManager();

            ScotiabankAdditionalInformation additionalInformation = new ScotiabankAdditionalInformation
            {
                CustomerNumber = "99576",
                ChargeAccount = "3200334804",
                FileNumberOfDay = "1",
                CompanyReference = "1234567899",
                PaymentDate = new DateTime(2020, 7, 4),
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
                Amount = 923.2m,
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
            employee.BankAccount = "03200770000";
            employee.FirstLastName = "ESCUDERO";
            employee.SecondLastName = "";
            employee.Name = "JULIA";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = scotiabankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overdraft()
        {
            //Arrange
            string expected = "EEHA9957601000000000000000000000000000                                                                                                                                                                                                                                                                                                                                            "
                               + Environment.NewLine
                               + "EEHB000000032003348041234567899000                                                                                                                                                                                                                                                                                                                                                "
                               + Environment.NewLine
                               + "EEDA040000000000009232020200704011                                ESCUDERO IGNACIO JULIA                  000000000000000100000000000000000000320077000000000                                        1 00000044044001NOMINA                                                                                                        0000000000000000000000000                      "
                               + Environment.NewLine
                               + "EEDA040000000000011622020200704012                                PEREZ DAVILA JANCARLOS                  000000000000000200000000000000000000320065119200000                                        1 00000044044001NOMINA                                                                                                        0000000000000000000000000                      "
                               + Environment.NewLine
                               + "EEDA040000000000009230020200704013                                CASTAÑEDA CERVANTES ROXANA GUADALUPE    000000000000000300000000000000000000320064914700000                                        1 00000044044001NOMINA                                                                                                        0000000000000000000000000                      "
                               + Environment.NewLine
                               + "EETB000000300000000000300840000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine
                               + "EETA000000300000000000300840000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000                                                                                                                           "
                               + Environment.NewLine;

            ScotiabankPaymentLayoutManager scotiabankLayout = new ScotiabankPaymentLayoutManager();

            ScotiabankAdditionalInformation additionalInformation = new ScotiabankAdditionalInformation
            {
                CustomerNumber = "99576",
                ChargeAccount = "3200334804",
                FileNumberOfDay = "1",
                CompanyReference = "1234567899",
                PaymentDate = new DateTime(2020, 7, 4),
            };

            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;

            //Employee 1
            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 923.2m,
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
            employee.BankAccount = "03200770000";
            employee.FirstLastName = "ESCUDERO";
            employee.SecondLastName = "IGNACIO";
            employee.Name = "JULIA";
            employee.Code = "002";
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
                Amount = 1200.0m,
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
                Amount = 120m,
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
                Amount = 45.0m,
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
                Amount = 112.8m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "03200651192";
            employee.FirstLastName = "PEREZ";
            employee.SecondLastName = "DAVILA";
            employee.Name = "JANCARLOS";
            employee.Code = "004";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Employee 3
            overdraftDetails = new List<OverdraftDetail>();

            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 1150.0m,
                Value = 7,
            };

            overdraftDetails.Add(detail);


            concept = new ConceptPayment
            {
                Name = "Todas deducciones",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 227.0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "BACF6710253V5";
            employee.BankAccount = "03200649147";
            employee.FirstLastName = "CASTAÑEDA";
            employee.SecondLastName = "CERVANTES";
            employee.Name = "ROXANA GUADALUPE";
            employee.Code = "00000007";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = scotiabankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);

        }
    }
}
