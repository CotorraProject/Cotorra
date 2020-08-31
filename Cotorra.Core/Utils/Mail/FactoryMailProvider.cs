using CotorraNode.Common.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Utils.Mail
{
    public enum SendMailProvider
    {
        SendGrid = 1
    }

    public static class FactoryMailProvider
    {
        private readonly static string mailProviderString = ConfigManager.GetValue("MailProvider");

        public static SendMailProvider GetProviderFromConfig()
        {
            Enum.TryParse(mailProviderString, out SendMailProvider sendMailProvider);
            return sendMailProvider;
        }

        public static ISendMailProvider CreateInstance(SendMailProvider sendMailProvider)
        {
            if (sendMailProvider == SendMailProvider.SendGrid)
            {
                return new SendGridProvider();
            }

            return null;
        }
    }
}
