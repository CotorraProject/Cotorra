using Cotorra.Core;
using System;
using Xunit;

namespace Cotorra.UnitTest
{
    public class PTUManagerUT
    {

        private decimal GetEmployeeWorkingDays(Guid employeeID, Guid instanceID, Guid companyID, DateTime initialDate, DateTime finalDate)
        {
            return 260M;
        }

        private decimal GetEmployeeAccumulatedSalary(Guid employeeID, Guid instanceID, Guid companyID, DateTime initialDate, DateTime finalDate)
        {
            return 58000M;
        }

        private decimal GetAllEmployeeWorkingDays(Guid instanceID, Guid companyID, DateTime initialDate, DateTime finalDate)
        {
            return 1630M;
        }

        private decimal GetAllEmployeeSalaries(Guid instanceID, Guid companyID, DateTime initialDate, DateTime finalDate)
        {
            return 360500M;
        }

        private decimal GetMinimunSalary(Guid employeeID, Guid instanceID, Guid companyID, string Zone)
        {
            return 123.22M;
        }

        [Fact]
        public void Should_CalculatePTU_ValidData()
        {
            /*
                Trabajador	    Días trabajados	    Sueldo total ganado en el año   PTU Antes de Impuestos
                Vendedor1	            260	            $58,000                        $208,257.95
                Vendedor2	            300	            $95,500
                Conserje	            360	            $50,000
                Secretaria	            350	            $77,000
                El poli de la entrada	360	            $80,000
                Director de finanzas	350	            $180,000
                TOTALES:	            1,630	        $360,500
             */

            //General
            var employeeID = Guid.NewGuid();
            var instanceID = Guid.NewGuid();
            var companyID = Guid.NewGuid();

            var ptuManager = new PtuManager();

            //1.Año para el calculo.
            var calculationYear = DateTime.UtcNow;
            var initialDate = new DateTime(calculationYear.Year, 1, 1);
            var finalDate = new DateTime(calculationYear.Year, 12, 31);

            //2. Importe PTU a repartir.
            var PTUToDistribute = 1300000;

            //3. Total de días trabajados (de TODOS los empleados).
            //1,630 días en total de los trabajadores
            var totalAllEmployeeWorkingDays = GetAllEmployeeWorkingDays(instanceID, companyID, initialDate, finalDate);
            Assert.True(totalAllEmployeeWorkingDays.Equals(1630M));

            //4. Total de sueldos pagados (de TODOS los empleados).
            var totalAllEmployeeSalaries = GetAllEmployeeSalaries(instanceID, companyID, initialDate, finalDate);
            Assert.True(totalAllEmployeeSalaries.Equals(360500M));

            //Calculation 
            //Calculation Step 1 - Days Factor - Factor de días = (50% de la PTU a repartir) /  Total de días (todos los empleados)
            var daysFactor = ptuManager.CalculateDaysFactor(PTUToDistribute, totalAllEmployeeWorkingDays);
            Assert.True(daysFactor.Equals(398.77300613496932515337423313M));

            //Calculation Step 2 - Salary Factor - Factor de Sueldos (50% de la PTU a repartir) / Total de sueldos pagados (todos los empleados)
            var salaryFactor = ptuManager.CalculateSalaryFactor(PTUToDistribute, totalAllEmployeeSalaries);
            Assert.True(salaryFactor.Equals(1.8030513176144244105409153953M));

            //Calculation per Employee
            //Step 3. Días trabajados del empleado (en todo el año).
            //Ejemplo: Vendedor 1: 260 días
            var employeeWorkingDays = GetEmployeeWorkingDays(employeeID, instanceID, companyID, initialDate, finalDate);
            Assert.True(employeeWorkingDays.Equals(260M));

            //Step 4. Sueldo acumulado del empleado (en todo el año).
            //Empleado: Vendedor 1: 58,000
            var employeeSalaryAccumulated = GetEmployeeAccumulatedSalary(employeeID, instanceID, companyID, initialDate, finalDate);
            Assert.True(employeeSalaryAccumulated.Equals(58000M));

            //Step 5. Factor de dias por empleado. ($103,680) - Factor de días de todos los trabajadores * Número de días trabajados por el empleado.
            //--A pagar por días
            var daysFactorEmployee = ptuManager.CalculateDayFactorEmployee(daysFactor, employeeWorkingDays);
            Assert.True(daysFactorEmployee.Equals(103680.98159509202453987730061M));

            //Step 6. Factor de sueldo por empleado. ($104,576.98) - Factor de sueldo de todos los trabajadores * Total de salario por empleado.
            //--A pagar por sueldos
            var salaryFactorEmployee = ptuManager.CalculateSalaryFactorEmployee(salaryFactor, employeeSalaryAccumulated);
            Assert.True(salaryFactorEmployee.Equals(104576.97642163661581137309293M));

            //Step 7. Total de PTU por conceptos de días y sueldos a pagar al empleado.  Factor de dias por empleado + Factor de sueldo por empleado.
            //($208,257.95) pesos de PTU
            var totalPTUEmployee = ptuManager.CalculateTotalPTU(daysFactorEmployee, salaryFactorEmployee);
            Assert.True(totalPTUEmployee.Equals(208257.95801672864035125039354M));

            //Calcular la parte excenta de PTU - 15 veces Salario Minimo de la Zona Geográfica del Empleado
            //123.22 pesos salario minimo 2020
            var maximumExcent = GetMinimunSalary(employeeID, instanceID, companyID, "A") * 15;
            Assert.True(maximumExcent.Equals(1848.3M));

            //Total de PTU Gravado = Total PTU - Maximo de excento
            var totalPTUGravado = totalPTUEmployee - maximumExcent;
            Assert.True(totalPTUGravado.Equals(208257.95801672864035125039354M - 1848.3M));

        }
    }
}
