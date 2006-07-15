using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using DBlog.TransitData;
using NHibernate;
using NHibernate.Expression;
using System.Web.Security;
using DBlog.Data;
using DBlog.Data.Hibernate;
using System.Collections.Generic;

namespace DBlog.WebServices
{
    /// <summary>
    /// DBlog Web Service
    /// </summary>
    [System.Web.Services.WebService(Namespace = "http://dblock.org/ns/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class Blog : VersionedWebService
    {
        public Blog()
        {

        }

        #region Logins

        static protected void CheckAdministrator(ISession session, string ticket)
        {
            if (!ManagedLogin.IsAdministrator(session, ticket))
            {
                throw new ManagedLogin.AccessDeniedException();
            }
        }

        /// <summary>
        /// Login to an account.
        /// </summary>
        /// <returns>login ticket</returns>
        [WebMethod(Description = "Login to an account.")]
        public string Login(string username, string password)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitLogin login = ManagedLogin.Login(session, username, password);
                HttpCookie cookie = FormsAuthentication.GetAuthCookie(login.Id.ToString(), false);
                return cookie.Value;
            }
        }

        /// <summary>
        /// Get a login.
        /// </summary>
        /// <returns>login ticket</returns>
        [WebMethod(Description = "Get a login.")]
        public TransitLogin GetLogin(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitLogin((DBlog.Data.Login)session.Load(typeof(DBlog.Data.Login),
                    ManagedLogin.GetLoginId(ticket)));
            }
        }

        [WebMethod(Description = "Get a login.")]
        public TransitLogin GetLoginById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                return new TransitLogin((Login)session.Load(typeof(Login), id));
            }
        }

        [WebMethod(Description = "Create or update a login.")]
        public int CreateOrUpdateLogin(string ticket, TransitLogin t_login)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Login login = t_login.GetLogin(session);
                session.SaveOrUpdate(login);
                session.Flush();
                return login.Id;
            }
        }

        [WebMethod(Description = "Get logins.")]
        public List<TransitLogin> GetLogins(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);

                ICriteria cr = session.CreateCriteria(typeof(Login));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitLogin> result = new List<TransitLogin>(list.Count);

                foreach (Login obj in list)
                {
                    result.Add(new TransitLogin(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a login.")]
        public void DeleteLogin(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                // TODO: check that not deleting last login
                session.Delete((Login)session.Load(typeof(Login), id));
                session.Flush();
            }
        }

        #endregion

        #region Topics

        [WebMethod(Description = "Get a topic.")]
        public TransitTopic GetTopicById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitTopic((Topic) session.Load(typeof(Topic), id));
            }
        }

        [WebMethod(Description = "Create or update a topic.")]
        public int CreateOrUpdateTopic(string ticket, TransitTopic t_topic)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Topic topic = t_topic.GetTopic(session);
                session.SaveOrUpdate(topic);
                session.Flush();
                return topic.Id;
            }
        }

        [WebMethod(Description = "Get topics.")]
        public List<TransitTopic> GetTopics(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ICriteria cr = session.CreateCriteria(typeof(Topic));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();
                
                List<TransitTopic> result = new List<TransitTopic>(list.Count);
                
                foreach (Topic obj in list)
                {
                    result.Add(new TransitTopic(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a topic.")]
        public void DeleteTopic(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Topic)session.Load(typeof(Topic), id));
                session.Flush();
            }
        }

        #endregion
    }
}