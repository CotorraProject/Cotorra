using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class SantanderPaymentLayoutManagerUT
    {
        [Fact]
        public void Should_Create_Layout_Single_Overdraft()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                         JIMENEZ             JOSE                          12345678901     000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "12345678901";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Jimenez";
            employee.Name = "JOSE";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_No_Second_Last_Name()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                                             JOSE                          12345678901     000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "12345678901";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = null;
            employee.Name = "JOSE";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_No_Employee_Bank_Account()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                         JIMENEZ             JOSE                                          000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = null;
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Jimenez";
            employee.Name = "JOSE";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Short_Employee_Bank_Account()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                         JIMENEZ             JOSE                          123456789       000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "123456789";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Jimenez";
            employee.Name = "JOSE";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Long_Employee_Bank_Account()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                         JIMENEZ             JOSE                          12345678901234567000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "12345678901234567";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Jimenez";
            employee.Name = "JOSE";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Long_Employee_Code()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001LOPEZ                         JIMENEZ             JOSE                          12345678901     000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "12345678901";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Jimenez";
            employee.Name = "JOSE";
            employee.Code = "000000001";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Long_Employee_Name()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000001BARRIENTOS DE DIOS DE LA CRUZ ARECHIGA DEL MONTE VMARIA JACARANDA ISABEL DE LA R12345678901     000000000000580780"
                               + Environment.NewLine
                               + "30000300001000000000000580780"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
                Amount = 6307.8m,
                Value = 15,
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

            //Deducctions
            concept = new ConceptPayment
            {
                Name = "IMSS",
                ConceptType = ConceptType.DeductionPayment,
            };

            detail = new OverdraftDetail
            {
                ConceptPayment = concept,
                Amount = 250.55m,
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
                Amount = 749.45m,
                Value = 0,
            };
            overdraftDetails.Add(detail);


            Employee employee = new Employee();
            Overdraft overdraft = new Overdraft();

            employee.RFC = "ROSY7611053X8";
            employee.BankAccount = "12345678901";
            employee.FirstLastName = "Barrientos de Dios de la Cruz Bendita";
            employee.SecondLastName = "Arechiga del Monte Verde";
            employee.Name = "Maria Jacaranda Isabel de la Reyna";
            employee.Code = "1";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_Create_Layout_Multiple_Overfrafts()
        {
            //Arrange
            string expected = "100001E0129202056000171742     01312020"
                               + Environment.NewLine
                               + "2000020000003ARRIAGA                       DARIO               JULIO                         11223344556     000000000000325580"
                               + Environment.NewLine
                               + "2000030000004VERA                          CASTRO              DOLORES                       12131415163     000000000000317400"
                               + Environment.NewLine
                               + "2000040000007LOPEZ                         SALAZAR             DANIEL                        12345678905     000000000000167200"
                               + Environment.NewLine
                               + "30000500003000000000000810180"
                               + Environment.NewLine;

            SantanderPaymentLayoutManager santanderLayout = new SantanderPaymentLayoutManager();
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
            employee.BankAccount = "11223344556";
            employee.FirstLastName = "Arriaga";
            employee.SecondLastName = "Dario";
            employee.Name = "Julio";
            employee.Code = "3";
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
            employee.BankAccount = "12131415163";
            employee.FirstLastName = "Vera";
            employee.SecondLastName = "Castro";
            employee.Name = "Dolores";
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
            employee.BankAccount = "12345678905";
            employee.FirstLastName = "Lopez";
            employee.SecondLastName = "Salazar";
            employee.Name = "Daniel";
            employee.Code = "00000007";
            overdraft.Employee = employee;
            overdraft.OverdraftDetails = overdraftDetails;

            overdrafts.Add(overdraft);

            SantanderAdditionalInformation additionalInformation = new SantanderAdditionalInformation
            {
                GenerationDate = new DateTime(2020, 1, 29),
                PaymentDate = new DateTime(2020, 1, 31),
                CompanyBankAccount = "56000171742",
            };

            //Act
            string actual = santanderLayout.GenerateLayout(overdrafts, additionalInformation);

            //Assert
            Assert.Equal(expected, actual);
        }

    }
}
