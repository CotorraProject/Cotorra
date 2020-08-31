using Cotorra.Core.Utils;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalStamping
{
    public interface IFiscalStamping
    {
        /// <summary>
        /// Create xml
        /// </summary>
        /// <typeparam name="ICFDINomProvider"></typeparam>
        /// <param name="oComprobante"></param>
        /// <returns></returns>
        string CreateXml<ICFDINomProvider>(ICFDINomProvider oComprobante, bool isTFD = false);

        /// <summary>
        /// Create comprobante
        /// </summary>
        /// <param name="payrollStampingParams"></param>
        /// <param name="payrollCompanyConfiguration"></param>
        /// <param name="overdraft"></param>
        /// <param name="overdraftResults"></param>
        /// <returns></returns>
        ICFDINomProvider CreateComprobante(CreateComprobanteParams createComprobanteParams);

        /// <summary>
        /// Sign Document
        /// </summary>
        /// <param name="cFDINomProvider"></param>
        /// <param name="certificateCER"></param>
        /// <param name="privateKey"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SignDocumentResult<ICFDINomProvider> SignDocument(ICFDINomProvider cFDINomProvider,
            byte[] certificateCER, byte[] privateKey, string password);

        /// <summary>
        /// StampDocument
        /// </summary>
        /// <param name="cFDINomProvider"></param>
        /// <returns></returns>
        Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(SignDocumentResult<ICFDINomProvider> signDocumentResult);
    }
}
