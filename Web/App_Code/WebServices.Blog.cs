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

        #endregion

        #region Topics

        [WebMethod(Description = "Get a topc.")]
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

                if (! ManagedLogin.IsAdministrator(session, ticket))
                {
                    throw new ManagedLogin.AccessDeniedException();
                }

                Topic topic = (t_topic.Id != 0) ? (Topic) session.Load(typeof(Topic), t_topic.Id) : new Topic();
                topic.Name = t_topic.Name;
                topic.Type = t_topic.Type;
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
                
                if (!ManagedLogin.IsAdministrator(session, ticket))
                {
                    throw new ManagedLogin.AccessDeniedException();
                }

                session.Delete((Topic)session.Load(typeof(Topic), id));
                session.Flush();
            }
        }

        #endregion
    }
}