using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeSalaryZoneFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeSalaryZoneFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int parameterIndex, double parameterValue)
        {
        }
        public double calculate()
        {
            var salaryZone = _functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone;
            if (salaryZone == SalaryZone.ZoneA)
            {
                return (double)'A';
            }
            else if (salaryZone == SalaryZone.ZoneB)
            {
                return (double)'B';
            }
            else
            {
                return (double)'C';
            }
        }
        public FunctionExtension clone()
        {
            return new EmployeeSalaryZoneFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}