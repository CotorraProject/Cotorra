using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class BBVAPaymentManagerUT
    {
        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "000000001REHE730714HG8   998899775533          000000000296020Ernesto Renteria Huerta                 001001"
                               + Environment.NewLine;

            BBVAPaymentLayoutManager bbvaLayout = new BBVAPaymentLayoutManager();
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
                Amount = 3200.0m,
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
                Amount = 239.8m,
                Value = 0,
            };

            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "8899775533";
            employee.FirstLastName = "Renteria";
            employee.SecondLastName = "Huerta";
            employee.Name = "Ernesto";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = bbvaLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Single_Overdraft_Long_Employee_Name()
        {
            //Arrange
            string expected = "000000001REHE730714HG8   998899775533          000000000296020Jose de Jesus de Dios Bendito Rodriguez 001001"
                               + Environment.NewLine;

            BBVAPaymentLayoutManager bbvaLayout = new BBVAPaymentLayoutManager();
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
                Amount = 3200.0m,
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
                Amount = 239.8m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "8899775533";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Garcia";
            employee.Name = "Jose de Jesus de Dios Bendito";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = bbvaLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overdrafts()
        {
            //Arrange
            string expected = "000000001REHE730714HG8   998899775533          000000000296020Ernesto Renteria Huerta                 001001"
                            + Environment.NewLine
                            + "000000002BACF6710256R4   997788556633          000000000748960Fermin Ballesteros Cruz                 001001"
                            + Environment.NewLine;

            BBVAPaymentLayoutManager bbvaLayout = new BBVAPaymentLayoutManager();
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
                Amount = 3200.0m,
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
                Amount = 239.8m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            //Employee 1
            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();
            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "8899775533";
            employee.FirstLastName = "Renteria";
            employee.SecondLastName = "Huerta";
            employee.Name = "Ernesto";
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
                Amount = 7399.9m,
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
                Amount = 650m,
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
                Amount = 239.8m,
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
                Amount = 320.5m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "BACF6710256R4";
            employee.BankAccount = "7788556633";
            employee.FirstLastName = "Ballesteros";
            employee.SecondLastName = "Cruz";
            employee.Name = "Fermin";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = bbvaLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
