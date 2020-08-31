using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class BanamexPaymentLayoutManagerUT
    {

        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "10227865814512007200001Version1300                         DEPOSITOS POR NOMINA05                                        B00"
                               + Environment.NewLine
                               + "2100100000000000009232001030100000000000896523174                  "
                               + Environment.NewLine
                               + "3000100000000000009232003055500000000002255669988002                                     Gonzalez Garcia Jose Luis                                                                                              1 03010000"
                               + Environment.NewLine
                               + "4001000001000000000000092320000001000000000000092320"
                               + Environment.NewLine;

            BanamexPaymentLayoutManager banamexbankLayout = new BanamexPaymentLayoutManager();

            BanamexAdditionalInformation additionalInformation = new BanamexAdditionalInformation
            {
                CustomerNumber = "22786581451",
                ChargeAccount = "896523174",
                FileNumberOfDay = "1",
                PaymentDate = new DateTime(2020, 7, 20),
                BranchOfficeNumber = "301",
                StateID = "1",
                CityID = "301",
                CompanyName = "Version1300"
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
            employee.BankAccount = "2255669988";
            employee.BankBranchNumber = "555";
            employee.FirstLastName = "Gonzalez";
            employee.SecondLastName = "Garcia";
            employee.Name = "Jose Luis";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banamexbankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_No_Second_Last_Name()
        {
            //Arrange
            string expected = "10227865814512007200001Version1300                         DEPOSITOS POR NOMINA05                                        B00"
                               + Environment.NewLine
                               + "2100100000000000009232001030100000000000896523174                  "
                               + Environment.NewLine
                               + "3000100000000000009232003055500000000002255669988002                                     Gonzalez  Jose Luis                                                                                                    1 03010000"
                               + Environment.NewLine
                               + "4001000001000000000000092320000001000000000000092320"
                               + Environment.NewLine;

            BanamexPaymentLayoutManager banamexbankLayout = new BanamexPaymentLayoutManager();

            BanamexAdditionalInformation additionalInformation = new BanamexAdditionalInformation
            {
                CustomerNumber = "22786581451",
                ChargeAccount = "896523174",
                FileNumberOfDay = "1",
                PaymentDate = new DateTime(2020, 7, 20),
                BranchOfficeNumber = "301",
                StateID = "1",
                CityID = "301",
                CompanyName = "Version1300"
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
            employee.BankAccount = "2255669988";
            employee.BankBranchNumber = "555";
            employee.FirstLastName = "Gonzalez";
            employee.SecondLastName = null;
            employee.Name = "Jose Luis";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banamexbankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_No_Employee_Branch_Office_Number()
        {
            //Arrange
            string expected = "10227865814512007200001Version1300                         DEPOSITOS POR NOMINA05                                        B00"
                               + Environment.NewLine
                               + "2100100000000000009232001030100000000000896523174                  "
                               + Environment.NewLine
                               + "3000100000000000009232003000000000000002255669988002                                     Gonzalez  Jose Luis                                                                                                    1 03010000"
                               + Environment.NewLine
                               + "4001000001000000000000092320000001000000000000092320"
                               + Environment.NewLine;

            BanamexPaymentLayoutManager banamexbankLayout = new BanamexPaymentLayoutManager();

            BanamexAdditionalInformation additionalInformation = new BanamexAdditionalInformation
            {
                CustomerNumber = "22786581451",
                ChargeAccount = "896523174",
                FileNumberOfDay = "1",
                PaymentDate = new DateTime(2020, 7, 20),
                BranchOfficeNumber = "301",
                StateID = "1",
                CityID = "301",
                CompanyName = "Version1300"
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
            employee.BankAccount = "2255669988";
            employee.BankBranchNumber = null;
            employee.FirstLastName = "Gonzalez";
            employee.SecondLastName = "";
            employee.Name = "Jose Luis";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banamexbankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Single_Overdraft_With_Kind_Concepts()
        {
            //Arrange
            string expected = "10227865814512007200001Version1300                         DEPOSITOS POR NOMINA05                                        B00"
                               + Environment.NewLine
                               + "2100100000000000009232001030100000000000896523174                  "
                               + Environment.NewLine
                               + "3000100000000000009232003055500000000002255669988002                                     Gonzalez Garcia Jose Luis                                                                                              1 03010000"
                               + Environment.NewLine
                               + "4001000001000000000000092320000001000000000000092320"
                               + Environment.NewLine;

            BanamexPaymentLayoutManager banamexbankLayout = new BanamexPaymentLayoutManager();

            BanamexAdditionalInformation additionalInformation = new BanamexAdditionalInformation
            {
                CustomerNumber = "22786581451",
                ChargeAccount = "896523174",
                FileNumberOfDay = "1",
                PaymentDate = new DateTime(2020, 7, 20),
                BranchOfficeNumber = "301",
                StateID = "1",
                CityID = "301",
                CompanyName = "Version1300"
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
                Name = "Percepción en especie",
                ConceptType = ConceptType.SalaryPayment,
                Kind = true,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 100.0m,
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
                Amount = 0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Subs causado",
                ConceptType = ConceptType.DeductionPayment,
                Kind = true,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 90m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = "2255669988";
            employee.BankBranchNumber = "555";
            employee.FirstLastName = "Gonzalez";
            employee.SecondLastName = "Garcia";
            employee.Name = "Jose Luis";
            employee.Code = "002";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);


            //Act
            string actual = banamexbankLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overdraft()
        {
            //Arrange
            string expected = "10227865814512007200001Version1300                         DEPOSITOS POR NOMINA05                                        B00"
                               + Environment.NewLine
                               + "2100100000000000119386001030100000000000896523174                  "
                               + Environment.NewLine
                               + "3000100000000000051662003055500000000002255669988002                                     Gonzalez Garcia Jose Luis                                                                                              1 03010000"
                               + Environment.NewLine
                               + "3000100000000000048042003033300000000000000007789006                                     Martinez Ruiz Juan Jose                                                                                                1 03010000"
                               + Environment.NewLine
                               + "3000100000000000019682003075400000000000071235489007                                     Funco Salon Demian                                                                                                     1 03010000"
                               + Environment.NewLine
                               + "4001000003000000000001193860000001000000000001193860"
                               + Environment.NewLine;

            BanamexPaymentLayoutManager scotiabankLayout = new BanamexPaymentLayoutManager();

            BanamexAdditionalInformation additionalInformation = new BanamexAdditionalInformation
            {
                CustomerNumber = "22786581451",
                ChargeAccount = "896523174",
                FileNumberOfDay = "1",
                PaymentDate = new DateTime(2020, 7, 20),
                BranchOfficeNumber = "301",
                StateID = "1",
                CityID = "301",
                CompanyName = "Version1300"
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
                Amount = 6000.0m,
                Value = 15,
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
                Amount = 833.8m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "POSL920306LD2";
            employee.BankAccount = "2255669988";
            employee.FirstLastName = "Gonzalez";
            employee.SecondLastName = "Garcia";
            employee.Name = "Jose Luis";
            employee.Code = "002";
            employee.BankBranchNumber = "555";
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
                Amount = 5423.45m,
                Value = 15,
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
                Amount = 100m,
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
                Amount = 134.94m,
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
                Amount = 584.31m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "7789";
            employee.FirstLastName = "Martinez";
            employee.SecondLastName = "Ruiz";
            employee.Name = "Juan Jose";
            employee.Code = "006";
            employee.BankBranchNumber = "333";
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
                Amount = 1950m,
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
                Amount = 45.07m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Subsidio",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = -63.27m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "BACF6710253V5";
            employee.BankAccount = "71235489";
            employee.FirstLastName = "Funco";
            employee.SecondLastName = "Salon";
            employee.Name = "Demian";
            employee.Code = "007";
            employee.BankBranchNumber = "754";
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
