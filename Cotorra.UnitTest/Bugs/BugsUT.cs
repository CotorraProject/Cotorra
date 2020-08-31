using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.ExpressionParser.Core.Cotorra;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest.Bugs
{
    public class BugsUT
    {
        /// <summary>
        /// Deducciones - Faltan cálculos de deducciones en el sobre recibo cuando el SBC es mayor a cero
        /// Abrir cualquier empresa
        ///Capturar un empleado con salario diario de $250 en periodo quincenal, 
        ///base cotización fija y SBC de $261.30
        ///Ir al sobre recibido del empleado y verificar deducciones
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TM_229_()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceID = Guid.NewGuid();

            var diarySalary = 250M;
            var fixedSBC = 261.30M;

            //creates overdraft
            var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(
                identityWorkId, instanceID, diarySalary, fixedSBC);

            ICalculateParams calculateByOverdraftParams = new Schema.Calculation.CalculateOverdraftParams()
            {
                OverdraftID = overdraft.ID,
                IdentityWorkID = identityWorkId,
                InstanceID = instanceID,
                ResetCalculation = true,
                UserID = Guid.NewGuid()
            };

            //Act
            ICalculationManager overdraftCalculationManager = new OverdraftCalculationManager();
            var calculateResult = await overdraftCalculationManager.CalculateAsync(calculateByOverdraftParams);
            var calculateOverdraftResult = calculateResult as CalculateOverdraftResult;
            var details = calculateOverdraftResult.OverdraftResult.OverdraftDetails;

            //Asserts
            //Ret. Inv. Y Vida - Deducción 5
            var retInvYVida = details.FirstOrDefault(p => 
                p.ConceptPayment.Code == 5 && 
                p.ConceptPayment.ConceptType == ConceptType.DeductionPayment);
            Assert.True(retInvYVida.Amount == 24.5M);

            //Ret. Cesantia - Deduccion 6
            var retCesantia = details.FirstOrDefault(p =>
                p.ConceptPayment.Code == 6 &&
                p.ConceptPayment.ConceptType == ConceptType.DeductionPayment);
            Assert.True(retCesantia.Amount == 44.09M);

        }
    }
}
