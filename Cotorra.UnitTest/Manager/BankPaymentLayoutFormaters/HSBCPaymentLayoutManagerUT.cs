using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class HSBCPaymentLayoutManagerUT
    {
        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "44112266770000002402320Abono por nómina              Rodriguez Sanchez Ivan                  "
                               + Environment.NewLine;

            HSBCPaymentLayoutManager hsbcLayout = new HSBCPaymentLayoutManager();
            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            //Salaries payments
            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 3000.0m,
                Value = 6,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Septimo día",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 500.0m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Reparto de utilidades",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 25825.31m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 148.3m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. Art142",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 4608.05m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. (mes)",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 545.88m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Ajuste al neto",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = -0.12m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "4411226677";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = hsbcLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Single_Overdraft_Long_Employee_Name()
        {
            //Arrange
            string expected = "44112266770000002402320Abono por nómina              Ballesteros Estilos Verdes De La Cruz De"
                               + Environment.NewLine;

            HSBCPaymentLayoutManager hsbcLayout = new HSBCPaymentLayoutManager();
            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            //Salaries payments
            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 3000.0m,
                Value = 6,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Septimo día",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 500.0m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Reparto de utilidades",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 25825.31m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 148.3m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. Art142",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 4608.05m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. (mes)",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 545.88m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Ajuste al neto",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = -0.12m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "4411226677";
            employee.FirstLastName = "Ballesteros Estilos Verdes";
            employee.SecondLastName = "De La Cruz De Dios";
            employee.Name = "Fermin Gonzalo De Jesus";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = hsbcLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Single_Overdraft_Short_BankAccount()
        {
            //Arrange
            string expected = "00889977550000002402320Abono por nómina              Ballesteros Estilos Verdes De La Cruz De"
                               + Environment.NewLine;

            HSBCPaymentLayoutManager hsbcLayout = new HSBCPaymentLayoutManager();
            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            //Salaries payments
            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 3000.0m,
                Value = 6,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Septimo día",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 500.0m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Reparto de utilidades",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 25825.31m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 148.3m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. Art142",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 4608.05m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. (mes)",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 545.88m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Ajuste al neto",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = -0.12m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "88997755";
            employee.FirstLastName = "Ballesteros Estilos Verdes";
            employee.SecondLastName = "De La Cruz De Dios";
            employee.Name = "Fermin Gonzalo De Jesus";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = hsbcLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Single_Overdraft_Long_BankAccount()
        {
            //Arrange
            string expected = "8899775533220000002402320Abono por nómina              Rodriguez Sanchez Ivan                  "
                               + Environment.NewLine;

            HSBCPaymentLayoutManager hsbcLayout = new HSBCPaymentLayoutManager();
            List<Overdraft> overdrafts = new List<Overdraft>();
            List<OverdraftDetail> overdraftDetails = new List<OverdraftDetail>();
            OverdraftDetail detail;
            ConceptPayment concept;


            //Salaries payments
            concept = new ConceptPayment
            {
                Name = "Sueldo",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 3000.0m,
                Value = 6,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Septimo día",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 500.0m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Reparto de utilidades",
                ConceptType = ConceptType.SalaryPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 25825.31m,
                Value = 1,
            };
            overdraftDetails.Add(detail);

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 148.3m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. Art142",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 4608.05m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "I.S.R. (mes)",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 545.88m,
                Value = 0,
            };
            overdraftDetails.Add(detail);

            concept = new ConceptPayment
            {
                Name = "Ajuste al neto",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = -0.12m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "889977553322";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = hsbcLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overfrafts()
        {
            //Arrange
            string expected = "44112266770000000325580Abono por nómina              Rodriguez Sanchez Ivan                  "
                            + Environment.NewLine
                            + "88997755440000000317400Abono por nómina              Renteria Huerta Ernesto                 "
                            + Environment.NewLine
                            + "77885566990000000167200Abono por nómina              Ballesteros Cruz Fermin Gonzalo         "
                            + Environment.NewLine;

            HSBCPaymentLayoutManager hsbcLayout = new HSBCPaymentLayoutManager();
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
                Amount = 3950.0m,
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
                Amount = 694.2m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            //Employee 1
            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();
            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "4411226677";
            employee.FirstLastName = "Rodriguez";
            employee.SecondLastName = "Sanchez";
            employee.Name = "Ivan";
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
                Amount = 3500.0m,
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
                Amount = 450m,
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
                Amount = 90.04m,
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
                Amount = 685.96m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "REHE730714HG8";
            employee.BankAccount = "8899775544";
            employee.FirstLastName = "Renteria";
            employee.SecondLastName = "Huerta";
            employee.Name = "Ernesto";
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
                Amount = 1850.0m,
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
                Amount = 178.0m,
                Value = 0,
            };

            overdraftDetails.Add(detail);

            employee = new Employee();
            overdraft = new Overdraft();
            employee.RFC = "BACF6710253V5";
            employee.BankAccount = "7788556699";
            employee.FirstLastName = "Ballesteros";
            employee.SecondLastName = "Cruz";
            employee.Name = "Fermin Gonzalo";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            //Act
            string actual = hsbcLayout.GenerateLayout(overdrafts, null);

            //Assert
            Assert.Equal(expected, actual);
        }


    }
}
