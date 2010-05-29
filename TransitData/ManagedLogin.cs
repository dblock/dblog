using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using DBlog.Data;
using System.Security.Cryptography;
using NHibernate;
using NHibernate.Criterion;
using System.Text;
using System.Web.Services.Protocols;
using System.Net.Mail;
using DBlog.Tools.Web;

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
            DBlog.Data.Login login = (DBlog.Data.Login)session.Load(typeof(DBlog.Data.Login), id);
            return ((TransitLoginRole)Enum.Parse(typeof(TransitLoginRole), login.Role) == TransitLoginRole.Administrator);
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

        #region Password

        public static string ResetPasswordEmail(ISession session, string usernameOrEmail)
        {
            Login login = session.CreateCriteria(typeof(Login))
                .Add(Expression.Eq("Email", usernameOrEmail))
                .UniqueResult<Login>();

            if (login == null)
            {
                login = session.CreateCriteria(typeof(Login))
                    .Add(Expression.Eq("Username", usernameOrEmail))
                    .UniqueResult<Login>();
            }

            if (login == null)
            {
                throw new Exception(string.Format("No user with e-mail or username '{0}' found.",
                    usernameOrEmail));
            }

            if (string.IsNullOrEmpty(login.Email))
            {
                throw new Exception(string.Format("User '{0}' does not have a registered e-mail address.",
                    usernameOrEmail));
            }

            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["Email"]);
            message.To.Add(new MailAddress(login.Email));
            message.Subject = string.Format("{0} Password Reset", ConfigurationManager.AppSettings["Url"]);
            message.IsBodyHtml = true;
            message.Body = string.Format("Please <a href='{0}ResetPassword.aspx?id={1}&username={2}&hash={3}'>click here</a> to reset your login password.",
                ConfigurationManager.AppSettings["Url"], login.Id, login.Email, Renderer.UrlEncode(GetPasswordHash(login.Password)));

            SmtpClient client = new SmtpClient();
            client.Send(message);
            return login.Email;
        }

        public static void ResetPassword(ISession session, int id, string hash, string newPassword)
        {
            Login login = session.Load<Login>(id);

            if (login == null)
            {
                throw new Exception(string.Format("Invalid login id '{0}'.", id));
            }

            if (GetPasswordHash(login.Password) != hash)
            {
                throw new Exception(string.Format("Invalid hash code '{0}'.", hash));
            }

            login.Password = GetPasswordHash(newPassword);
            session.SaveOrUpdate(login);
            session.Flush();
        }

        #endregion
    }
}