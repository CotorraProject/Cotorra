using MoreLinq.Extensions;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    public interface ISettlementProcessManagerLetterHelper
    {

        /// <summary>
        /// Generates the settlement letter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters, IMSSpreadsheetWriter writer);

        /// <summary>
        /// Generates the settlement letter indicates writer.
        /// </summary>
        /// <param name="activeOverdraft">The active overdraft.</param>
        /// <param name="identityWorkID">The identity work identifier.</param>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        Task<string> GenerateSettlementLetter(List<Overdraft> activeOverdrafts, Guid identityWorkID, Guid instanceID, string token, IMSSpreadsheetWriter writer);

       
    }

}
