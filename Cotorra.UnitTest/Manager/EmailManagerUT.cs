using System;
using System.Collections.Generic;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Xunit;
using Cotorra.Core.Utils.Mail;
using Cotorra.Schema;
using System.IO;

namespace Cotorra.UnitTest
{
    public class EmailManagerUT
    {
        [Fact]
        public async Task SendEmail_TXTTM()
        {
            ISendMailProvider sendMailProvider = FactoryMailProvider.CreateInstance(FactoryMailProvider.GetProviderFromConfig());
            var sendMailParams = new Schema.SendMailParams()
            {
                HTMLContent = "<strong>Este es un correo de prueba</strong>",
                PlainContentText = "Este es un correo de prueba",
                Subject = "Cotorria entrega CFDI"
            };

            var bytesContent = File.ReadAllBytes(Path.Combine(EmployerRegistrationUT.AssemblyDirectory, "testingFiles\\example.txt"));
            sendMailParams.SendMailAddresses.Add(new Schema.SendMailAddress() { Email = "hector.ramirez@cotorrai.com", Name = "Omar" });
            sendMailParams.SendMailAttachments.Add(new Schema.SendMailAttachment() { Attachment = bytesContent, Filename = "cfdi.txt", TypeAttachment = TypeAttachment.TXT });

            await sendMailProvider.SendMailAsync(sendMailParams);
        }

        [Fact]
        public async Task SendEmail_PDFTM()
        {
            ISendMailProvider sendMailProvider = FactoryMailProvider.CreateInstance(FactoryMailProvider.GetProviderFromConfig());
            var sendMailParams = new Schema.SendMailParams()
            {
                HTMLContent = "<strong>Este es un correo de prueba</strong>",
                PlainContentText = "Este es un correo de prueba",
                Subject = "Cotorria entrega CFDI"
            };

            var bytesContent = File.ReadAllBytes(Path.Combine(EmployerRegistrationUT.AssemblyDirectory, "testingFiles\\example.pdf"));
            sendMailParams.SendMailAddresses.Add(new Schema.SendMailAddress() { Email = "hector.ramirez@cotorrai.com", Name = "Omar" });
            sendMailParams.SendMailAttachments.Add(new Schema.SendMailAttachment() { Attachment = bytesContent, Filename = "cfdi.pdf", TypeAttachment = TypeAttachment.PDF });

            await sendMailProvider.SendMailAsync(sendMailParams);
        }

        [Fact]
        public async Task SendEmail_XMLTM()
        {
            ISendMailProvider sendMailProvider = FactoryMailProvider.CreateInstance(FactoryMailProvider.GetProviderFromConfig());
            var sendMailParams = new Schema.SendMailParams()
            {
                HTMLContent = "<strong>Este es un correo de prueba</strong>",
                PlainContentText = "Este es un correo de prueba",
                Subject = "Cotorria entrega CFDI"
            };

            var bytesContent = File.ReadAllBytes(Path.Combine(EmployerRegistrationUT.AssemblyDirectory, "testingFiles\\example.xml"));
            sendMailParams.SendMailAddresses.Add(new Schema.SendMailAddress() { Email = "hector.ramirez@cotorrai.com", Name = "Omar" });
            sendMailParams.SendMailAttachments.Add(new Schema.SendMailAttachment() { Attachment = bytesContent, Filename = "cfdi.xml", TypeAttachment = TypeAttachment.XML });

            await sendMailProvider.SendMailAsync(sendMailParams);
        }
    }
}
