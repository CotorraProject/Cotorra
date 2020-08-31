using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils.Mail
{
    public interface ISendMailProvider
    {
        Task SendMailAsync(SendMailParams sendMailParams);
    }
}
