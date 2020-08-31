using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CotorraNode.App.Common.UX;
using CotorraNode.Common.Config;
using Microsoft.AspNetCore.Http;

namespace Cotorra.Web.Utils
{
    public static class SessionModel
    {
        private static String _acsService = ConfigManager.GetValue("ACSService");

        public static void Initialize()
        {
            try
            {
                if (SessionModel.IsActive)
                {
                    return;
                }

                //Get main headers to obtain session
                var gid = System.Web.HttpContext.Current.Request.Headers["gid"];
                var iid = System.Web.HttpContext.Current.Request.Headers["iid"];
                var ssid = System.Web.HttpContext.Current.Request.Headers["ssid"];

                //No headers, no fun
                if (String.IsNullOrEmpty(gid) || String.IsNullOrEmpty(iid) || String.IsNullOrEmpty(ssid))
                {
                    throw new Exception("NOSESSION (NO HEADERS). RESTART APPLICATION.");
                }

                //Set keys
                var guidID = SecurityUX.DecryptString(gid.ToString(), "");
                var identityID = Guid.Parse(SecurityUX.DecryptString(iid.ToString(), guidID.ToString()));
                var sessionID = Guid.Parse(SecurityUX.DecryptString(ssid.ToString(), guidID.ToString()));

                //Get session from cache o redis
                var session = SessionUtils.GetSession(sessionID.ToString(), identityID.ToString());

                if (session.Session == null)
                {
                    throw new Exception("NOSESSION (NO REDIS). RESTART APPLICATION.");
                }

                //Validate session expiration
                var expiresOn = new DateTime(long.Parse(session.Session.Attributes["ExpiresOn"].ToString()));
                if (DateTime.UtcNow > expiresOn)
                {
                    throw new Exception("SESSIONEXPIRED. RESTART APPLICATION.");
                }

                //Save session on current http context
                SessionModel.SessionID = sessionID;
                SessionModel.GuidID = Guid.Parse(guidID);
                SessionModel.IdentityID = identityID;
                SessionModel.AuthorizationHeader = session.Session.Attributes["AuthorizationHeader"].ToString();
                SessionModel.TFAPin = session.Session.Attributes["2FAPin"].ToString();
                SessionModel.Username = session.Session.Attributes["UPN"].ToString();
                SessionModel.FingerPrint = session.Session.Attributes["FingerPrint"].ToString();
                SessionModel.DisplayName = session.Session.Attributes["DisplayName"].ToString();
                SessionModel.ExpirationDateTime = session.Session.Attributes["ExpiresOn"].ToString();
                SessionModel.IsActive = true;
                SessionModel.Attributes = new Dictionary<String, String>();
                foreach (var item in session.Session.Attributes)
                {
                    SessionModel.Attributes[item.Name.ToString()] = item.Value.ToString();
                }

                //Set Culture
                Thread.CurrentThread.CurrentCulture = new CultureInfo(SessionModel.Culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(SessionModel.Culture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<String, String> Attributes
        {
            get
            {
                return (Dictionary<String, String>)System.Web.HttpContext.Current.Items["sattr"];
            }

            set
            {
                System.Web.HttpContext.Current.Items["sattr"] = value;
            }
        }

        public static Guid SessionID
        {
            get
            {
                return Guid.Parse(System.Web.HttpContext.Current.Items["ssid"].ToString());
            }

            set
            {
                System.Web.HttpContext.Current.Items["ssid"] = value.ToString();
            }
        }

        public static Guid IdentityID
        {
            get
            {
                return Guid.Parse(System.Web.HttpContext.Current.Items["iid"].ToString());
            }

            set
            {
                System.Web.HttpContext.Current.Items["iid"] = value.ToString();
            }
        }

        public static Guid GuidID
        {
            get
            {
                return Guid.Parse(System.Web.HttpContext.Current.Items["gid"].ToString());
            }

            set
            {
                System.Web.HttpContext.Current.Items["gid"] = value.ToString();
            }
        }

        public static String AuthorizationHeader
        {
            get
            {
                return System.Web.HttpContext.Current.Items["ah"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["ah"] = value.ToString();
            }
        }

        public static String TFAPin
        {
            get
            {
                return System.Web.HttpContext.Current.Items["2FAPin"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["2FAPin"] = value.ToString();
            }
        }

        public static Boolean Has2FA
        {
            get
            {
                return !String.IsNullOrEmpty(System.Web.HttpContext.Current.Items["2FAPin"].ToString());
            }
        }

        public static String Username
        {
            get
            {
                return System.Web.HttpContext.Current.Items["un"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["un"] = value.ToString();
            }
        }

        public static String FingerPrint
        {
            get
            {
                return System.Web.HttpContext.Current.Items["fpid"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["fpid"] = value.ToString();
            }
        }

        public static String DisplayName
        {
            get
            {
                return System.Web.HttpContext.Current.Items["dn"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["dn"] = value.ToString();
            }
        }

        public static String EncryptKey
        {
            get
            {
                return GuidID.ToString();
                //return Has2FA ? TFAPin : GuidID.ToString();
            }
        }

        public static String ExpirationDateTime
        {
            get
            {
                return System.Web.HttpContext.Current.Items["eo"].ToString();
            }

            set
            {
                System.Web.HttpContext.Current.Items["eo"] = value.ToString();
            }
        }

        public static String Culture
        {
            get
            {
                return "es-MX";
                //return System.Web.HttpContext.Current.Session.GetString("Culture");
            }

            set
            {
                System.Web.HttpContext.Current.Session.SetString("Culture", value);
            }
        }

        public static Boolean IsActive
        {
            get
            {
                if (System.Web.HttpContext.Current.Items["isa"] != null)
                {
                    return Boolean.Parse(System.Web.HttpContext.Current.Items["isa"].ToString());
                }
                return false;
            }

            set
            {
                System.Web.HttpContext.Current.Items["isa"] = value.ToString();
            }
        }

        /***************/
        /*HTTPS Headers*/
        /***************/

        public static Guid CompanyID
        {
            get
            {
                //No company ID selected
                if (String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["cid"]))
                {
                    return Guid.Empty;
                }

                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["cid"], SessionModel.EncryptKey));
            }
        }

        public static Guid InstanceID
        {
            get
            {
                //No instance ID selected
                if (String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["inid"]))
                {
                    return Guid.Empty;
                }

                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["inid"], SessionModel.EncryptKey));
            }
        }

        public static Guid LicenseServiceID
        {
            get
            {
                //No LicenseServiceID selected
                if (String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["lsid"]))
                {
                    return Guid.Empty;
                }

                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["lsid"], SessionModel.EncryptKey));
            }
        }

        public static Guid LicenseID
        {
            get
            {
                //No LicenseServiceID selected
                if (String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Headers["lid"]))
                {
                    return Guid.Empty;
                }

                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["lid"], SessionModel.EncryptKey));
            }
        }

        public static Guid ServiceID
        {
            get
            {
                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["srid"], SessionModel.EncryptKey));
            }
        }

        public static Guid AppID
        {
            get
            {
                return new Guid(SecurityUX.DecryptString(System.Web.HttpContext.Current.Request.Headers["aid"], SessionModel.EncryptKey));
            }
        }

        public static string RFC
        {
            get
            {
                return System.Web.HttpContext.Current.Request.Headers["rfc"];
            }
        }

    }
}
