using System;

namespace Cotorra.Core
{
    /// <summary>
    /// All operations and calculations about the utilities
    /// </summary>
    public class PtuManager
    {
        /// <summary>
        /// Half part or the PTU
        /// </summary>
        private const decimal PTU_PROPORTION = 0.5M;

        /// <summary>
        /// Calculate days factor.
        /// </summary>
        /// <param name="PTUToDistribute">Ammount of PTU to Distribute</param>
        /// <param name="totalAllEmployeeWorkingDays">Total Working days</param>
        /// <returns></returns>
        public decimal CalculateDaysFactor(decimal PTUToDistribute, decimal totalWorkingDays)
        {
            var daysFactor = (PTUToDistribute * PTU_PROPORTION) / (totalWorkingDays);
            return daysFactor;
        }

        /// <summary>
        /// Calculate salaries factor.
        /// </summary>
        /// <param name="PTUToDistribute">>Ammount of PTU to Distribute</param>
        /// <param name="totalSalaries">Total Salary</param>
        /// <returns></returns>
        public decimal CalculateSalaryFactor(decimal PTUToDistribute, decimal totalSalaries)
        {
            var salaryFactor = (PTUToDistribute * PTU_PROPORTION) / (totalSalaries);
            return salaryFactor;
        }

        /// <summary>
        /// Calculate day factor of employee
        /// </summary>
        /// <param name="daysFactor">Days factor</param>
        /// <param name="employeeWorkingDays">Employee working days</param>
        /// <returns></returns>
        public decimal CalculateDayFactorEmployee(decimal daysFactor, decimal employeeWorkingDays)
        {
            var employeedayFactor = daysFactor * employeeWorkingDays;
            return employeedayFactor;
        }

        /// <summary>
        /// Calculate salary factor of employee
        /// </summary>
        /// <param name="salaryFactor">Salary factor</param>
        /// <param name="employeeSalaryAccumulated">Employee acumulated salary</param>
        /// <returns></returns>
        public decimal CalculateSalaryFactorEmployee(decimal salaryFactor, decimal employeeSalaryAccumulated)
        {
            var employeeSalaryFactor = salaryFactor * employeeSalaryAccumulated;
            return employeeSalaryFactor;
        }

        /// <summary>
        /// Calculate total ptu of employee
        /// </summary>
        /// <param name="salaryFactorEmployee">Salary factor of employee</param>
        /// <param name="dayFactorEmployee">Day factor of employee</param>
        /// <returns></returns>
        public decimal CalculateTotalPTU(decimal salaryFactorEmployee, decimal dayFactorEmployee)
        {
            var employeeSalaryFactor = salaryFactorEmployee + dayFactorEmployee;
            return employeeSalaryFactor;
        }
    }
}
