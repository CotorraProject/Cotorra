using System;
using System.Net.Mail;

namespace Cotorra.Web.Utils
{
    public static class EmailUtil
    {
        public static bool IsValid(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);                
                return !String.IsNullOrEmpty(m.Address);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
