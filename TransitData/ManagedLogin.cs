using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using DBlog.Data;
using System.Security.Cryptography;
using NHibernate;
using NHibernate.Expression;
using System.Text;
using System.Web.Services.Protocols;

namespace DBlog.TransitData
{
    public class ManagedLogin
    {
        public class AccessDeniedException : SoapException
        {
            public AccessDeniedException()
                : base("Access denied", SoapException.ClientFaultCode)
            {

            }
        }

        public static bool IsAdministrator(ISession session, string ticket)
        {
            if (string.IsNullOrEmpty(ticket)) return false;
            int id = GetLoginId(ticket);
            DBlog.Data.Login login = (DBlog.Data.Login) session.Load(typeof(DBlog.Data.Login), id);
            return ((TransitLoginRole) Enum.Parse(typeof(TransitLoginRole), login.Role) == TransitLoginRole.Administrator);
        }

        public static int GetLoginId(string ticket)
        {
            FormsAuthenticationTicket t = FormsAuthentication.Decrypt(ticket);
            if (t == null)
            {
                throw new AccessDeniedException();
            }

            return int.Parse(t.Name);
        }

        #region Login

        public static TransitLogin Login(ISession session, string username, string password)
        {
            return LoginMd5(session, username, GetPasswordHash(password));
        }

        public static TransitLogin LoginMd5(ISession session, string username, string passwordhash)
        {
            // find a verified e-mail associated with an Login with the same password
            DBlog.Data.Login l = (DBlog.Data.Login)session.CreateCriteria(typeof(DBlog.Data.Login))
                    .Add(Expression.Eq("Username", username))
                    .UniqueResult();

            if (l == null || l.Password != passwordhash)
            {
                throw new AccessDeniedException();
            }

            return new TransitLogin(l);
        }

        public static string GetPasswordHash(string password)
        {
            return Convert.ToBase64String(
                new MD5CryptoServiceProvider().ComputeHash(
                    Encoding.Default.GetBytes(password)));
        }

        #endregion
    }
}