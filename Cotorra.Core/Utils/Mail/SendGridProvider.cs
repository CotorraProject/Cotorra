using CotorraNode.Common.Config;
using Cotorra.Schema;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils.Mail
{
    /// <summary>
    /// SendGrid Mail Provider
    /// userSendGridCotorria
    /// P#r$0n1A2020.!
    /// </summary>
    public class SendGridProvider : ISendMailProvider
    {
        #region "Attributes"
        private readonly string _sendGridAPIKey;
        private readonly string _deliveryAccountEmail;
        private readonly string _deliveryAccountDescription;

        private const string TXT_TYPE_ATTACHMENT = "txt/plain";
        private const string DISPOSITION = "attachment";
        private const string SENDMAIL_GENERIC_EXCEPTION = "Ocurrió un error al mandar el correo electrónico";
        #endregion

        #region "Constructor"
        public SendGridProvider()
        {
            _sendGridAPIKey = ConfigManager.GetValue("SendGridAPIKey");
            _deliveryAccountEmail = ConfigManager.GetValue("DeliveryAccountEmail");
            _deliveryAccountDescription = ConfigManager.GetValue("DeliveryAccountDescription");
        }
        #endregion

        public async Task SendMailAsync(SendMailParams sendMailParams)
        {
            var client = new SendGridClient(_sendGridAPIKey);
            var from = new EmailAddress(_deliveryAccountEmail, _deliveryAccountDescription);
            var subject = sendMailParams.Subject;
            var tos = new List<EmailAddress>();

            sendMailParams.SendMailAddresses.ForEach(p =>
            {
                tos.Add(new EmailAddress(p.Email, p.Name));
            });

            var plainTextContent = sendMailParams.PlainContentText;
            var htmlContent = sendMailParams.HTMLContent;
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent);

            if (sendMailParams.SendMailAttachments.Any())
            {
                msg.Attachments = new List<Attachment>();

                //Example attachment
                sendMailParams.SendMailAttachments.ForEach(sendMailAttachment =>
                {
                    var typeOfAttachment = String.Empty;
                    if (sendMailAttachment.TypeAttachment == TypeAttachment.PDF)
                    {
                        typeOfAttachment = System.Net.Mime.MediaTypeNames.Application.Pdf;
                    }
                    else if (sendMailAttachment.TypeAttachment == TypeAttachment.XML)
                    {
                        typeOfAttachment = System.Net.Mime.MediaTypeNames.Application.Xml;
                    }
                    else if (sendMailAttachment.TypeAttachment == TypeAttachment.TXT)
                    {
                        typeOfAttachment = TXT_TYPE_ATTACHMENT;
                    }

                    var attachment = new SendGrid.Helpers.Mail.Attachment
                    {
                        Content = Convert.ToBase64String(sendMailAttachment.Attachment),
                        Filename = sendMailAttachment.Filename,
                        Type = typeOfAttachment,
                        Disposition = DISPOSITION
                    };
                    msg.Attachments.Add(attachment);
                });
            }

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new CotorraException(9101, "9101", SENDMAIL_GENERIC_EXCEPTION, null);
            }
        }
    }
}
