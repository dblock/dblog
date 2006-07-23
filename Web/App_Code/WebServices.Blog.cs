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

                if (t_login.Role != TransitLoginRole.Administrator && t_login.Id == ManagedLogin.GetLoginId(ticket))
                {
                    // check whether self and administrator
                    throw new InvalidOperationException("Cannot Demote Self");    
                }

                session.SaveOrUpdate(login);
                session.Flush();
                return login.Id;
            }
        }

        [WebMethod(Description = "Get logins count.")]
        public int GetLoginsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                return new CountQuery(session, typeof(DBlog.Data.Login), "Login").Execute();
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

        [WebMethod(Description = "Get topics count.")]
        public int GetTopicsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Topic), "Topic").Execute();
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

        #region Blogs (Combined Entries and Galleries)

        [WebMethod(Description = "Get a blog.")]
        public TransitBlog GetBlogById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitBlog(session, (DBlog.Data.Blog)session.Load(typeof(DBlog.Data.Blog), id));
            }
        }

        [WebMethod(Description = "Get blogs count.")]
        public int GetBlogsCount(string ticket, TransitBlogQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery q = new CountQuery(session, typeof(DBlog.Data.Blog), "Blog");
                if (options != null) options.Apply(q);
                return q.Execute();
            }
        }

        [WebMethod(Description = "Get blogs.")]
        public List<TransitBlog> GetBlogs(string ticket, TransitBlogQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ICriteria cr = session.CreateCriteria(typeof(DBlog.Data.Blog));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitBlog> result = new List<TransitBlog>(list.Count);

                foreach (DBlog.Data.Blog obj in list)
                {
                    result.Add(new TransitBlog(session, obj));
                }

                return result;
            }
        }

        #endregion

        #region Images

        [WebMethod(Description = "Create or update an image.")]
        public int CreateOrUpdateImage(string ticket, TransitImage t_image)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Image image = t_image.GetImage(session);
                session.SaveOrUpdate(image);
                session.Flush();
                return image.Id;
            }
        }

        [WebMethod(Description = "Get an image.")]
        public TransitImage GetImageById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id));
            }
        }

        [WebMethod(Description = "Delete an image.")]
        public void DeleteImage(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete(session.Load(typeof(DBlog.Data.Image), id));
                session.Flush();
            }
        }

        [WebMethod(Description = "Get images count.")]
        public int GetImagesCount(string ticket, TransitImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Image), "Image");
                if (options != null) options.Apply(query);
                return query.Execute();
            }
        }

        [WebMethod(Description = "Get images.")]
        public List<TransitImage> GetImages(string ticket, TransitImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Image));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitImage> result = new List<TransitImage>(list.Count);

                foreach (Image obj in list)
                {
                    result.Add(new TransitImage(obj));
                }

                return result;
            }
        }

        #endregion

        #region Images with Bitmaps

        [WebMethod(Description = "Get image data.", BufferResponse = true)]
        public TransitImage GetImageWithBitmapById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), false, true);
            }
        }

        [WebMethod(Description = "Get image data if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithBitmapByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitImage img = new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id));

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), false, true);
            }
        }

        [WebMethod(Description = "Get image thumbnail.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), true, false); 
            }
        }

        [WebMethod(Description = "Get image thumbnail if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            // todo: check permissions with ticket
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitImage img = new TransitImage((DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id));

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage((DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), true, false);
            }
        }

        #endregion

        #region References

        [WebMethod(Description = "Get a reference.")]
        public TransitReference GetReferenceById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitReference((Reference)session.Load(typeof(Reference), id));
            }
        }

        [WebMethod(Description = "Get a reference.")]
        public TransitReference GetReferenceByWord(string ticket, string word)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                Reference reference = (Reference)session.CreateCriteria(typeof(Reference))
                    .Add(Expression.Eq("Word", word)).UniqueResult();
                if (reference == null) return null;
                return new TransitReference(reference);
            }
        }


        [WebMethod(Description = "Create or update a reference.")]
        public int CreateOrUpdateReference(string ticket, TransitReference t_reference)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Reference reference = t_reference.GetReference(session);
                session.SaveOrUpdate(reference);
                session.Flush();
                return reference.Id;
            }
        }

        [WebMethod(Description = "Get references count.")]
        public int GetReferencesCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Reference), "Reference").Execute();
            }
        }

        [WebMethod(Description = "Get references.")]
        public List<TransitReference> GetReferences(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Reference));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitReference> result = new List<TransitReference>(list.Count);

                foreach (Reference obj in list)
                {
                    result.Add(new TransitReference(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a reference.")]
        public void DeleteReference(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Reference)session.Load(typeof(Reference), id));
                session.Flush();
            }
        }

        #endregion

        #region Referrer Host Rollups

        [WebMethod(Description = "Get a referrer host rollup.")]
        public TransitReferrerHostRollup GetReferrerHostRollupById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitReferrerHostRollup((ReferrerHostRollup)session.Load(typeof(ReferrerHostRollup), id));
            }
        }

        [WebMethod(Description = "Create or update a referrer host rollup.")]
        public int CreateOrUpdateReferrerHostRollup(string ticket, TransitReferrerHostRollup t_referrerhostrollup)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                ReferrerHostRollup referrerhostrollup = t_referrerhostrollup.GetReferrerHostRollup(session);
                session.SaveOrUpdate(referrerhostrollup);
                session.Flush();
                return referrerhostrollup.Id;
            }
        }

        [WebMethod(Description = "Get referrer host rollups count.")]
        public int GetReferrerHostRollupsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.ReferrerHostRollup), "ReferrerHostRollup").Execute();
            }
        }

        [WebMethod(Description = "Get referrer host rollups.")]
        public List<TransitReferrerHostRollup> GetReferrerHostRollups(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(ReferrerHostRollup));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitReferrerHostRollup> result = new List<TransitReferrerHostRollup>(list.Count);

                foreach (ReferrerHostRollup obj in list)
                {
                    result.Add(new TransitReferrerHostRollup(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a referrer host rollup.")]
        public void DeleteReferrerHostRollup(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((ReferrerHostRollup)session.Load(typeof(ReferrerHostRollup), id));
                session.Flush();
            }
        }

        #endregion

        #region Highlights

        [WebMethod(Description = "Get a highlight.")]
        public TransitHighlight GetHighlightById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitHighlight((Highlight)session.Load(typeof(Highlight), id));
            }
        }

        [WebMethod(Description = "Create or update a highlight.")]
        public int CreateOrUpdateHighlight(string ticket, TransitHighlight t_highlight)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Highlight highlight = t_highlight.GetHighlight(session);
                session.SaveOrUpdate(highlight);
                session.Flush();
                return highlight.Id;
            }
        }

        [WebMethod(Description = "Get highlights count.")]
        public int GetHighlightsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Highlight), "Highlight").Execute();
            }
        }

        [WebMethod(Description = "Get highlights.")]
        public List<TransitHighlight> GetHighlights(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Highlight));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

                List<TransitHighlight> result = new List<TransitHighlight>(list.Count);

                foreach (Highlight obj in list)
                {
                    result.Add(new TransitHighlight(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a highlight.")]
        public void DeleteHighlight(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Highlight)session.Load(typeof(Highlight), id));
                session.Flush();
            }
        }

        #endregion

    }
}