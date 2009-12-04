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
using System.Reflection;
using DBlog.TransitData.References;
using System.Text;
using DBlog.Tools.Drawing.Exif;
using DBlog.Tools.Web;
using System.IO;

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

        [WebMethod(Description = "Get a login by username.")]
        public TransitLogin GetLoginByUsername(string ticket, string username)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Login login = session.CreateCriteria(typeof(Login))
                    .Add(Expression.Eq("Username", username))
                    .UniqueResult<Login>();

                if (login == null)
                {
                    throw new Exception("User Not Found");
                }

                return new TransitLogin(login);
            }
        }

        [WebMethod(Description = "Send an e-mail to reset the login password.")]
        public string ResetLoginPasswordEmail(string usernameOrEmail)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return ManagedLogin.ResetPasswordEmail(session, usernameOrEmail);
            }
        }

        [WebMethod(Description = "Reset the login password.")]
        public void ResetLoginPassword(int id, string hash, string newPassword)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ManagedLogin.ResetPassword(session, id, hash, newPassword);
            }
        }

        [WebMethod(Description = "Create or update a login.")]
        public int CreateOrUpdateLogin(string ticket, TransitLogin t_login)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                Login login = t_login.GetLogin(session);

                if (!ManagedLogin.IsAdministrator(session, ticket) && t_login.Role == TransitLoginRole.Administrator)
                {
                    throw new ManagedLogin.AccessDeniedException();
                }

                if (ManagedLogin.IsAdministrator(session, ticket))
                {
                    if (t_login.Role != TransitLoginRole.Administrator && t_login.Id == ManagedLogin.GetLoginId(ticket))
                    {
                        // check whether self and administrator
                        throw new InvalidOperationException("Cannot Demote Self");
                    }
                }

                // find an existing login, if any
                Login existingLogin = session.CreateCriteria(typeof(Login))
                        .Add(Expression.Eq("Username", login.Username))
                        .Add(Expression.Not(Expression.Eq("Id", login.Id)))
                        .UniqueResult<Login>();

                if (existingLogin != null)
                {
                    throw new Exception(string.Format("A user registered with e-mail {0} already exists. Please choose a different one.",
                        login.Email));
                }

                session.SaveOrUpdate(login);
                session.Flush();
                return login.Id;
            }
        }

        [WebMethod(Description = "Get logins count.")]
        public int GetLoginsCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Login), "Login");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
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

                IList<Login> list = cr.List<Login>();

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
                
                // check deleting self - since self must be an admin we never delete the last admin either
                int self_id = ManagedLogin.GetLoginId(ticket);
                if (self_id == id)
                {
                    throw new InvalidOperationException("Cannot Delete Self");
                }

                Login login = (Login)session.Load(typeof(Login), id);

                // delete post logins
                if (login.PostLogins != null)
                {
                    foreach (PostLogin pl in login.PostLogins)
                    {
                        session.Delete(pl);
                    }
                }

                // delete login counters
                if (login.LoginCounters != null)
                {
                    foreach (LoginCounter c in login.LoginCounters)
                    {
                        session.Delete(c);
                        session.Delete(c.Counter);
                    }
                }

                // update post ownership created by this login to the current user
                if (login.Posts != null)
                {
                    Login self = (Login) session.Load(typeof(Login), self_id);
                    foreach (Post p in login.Posts)
                    {
                        p.Login = self;
                        session.Save(p);
                    }
                }

                if (login.Comments != null)
                {
                    foreach (Comment c in login.Comments)
                    {
                        c.OwnerLogin = null;
                        session.Save(c);
                    }
                }

                session.Delete(login);
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
        public int GetTopicsCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Topic), "Topic");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
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

                IList<Topic> list = cr.List<Topic>();
                
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

        #region Images

        [WebMethod(Description = "Create or update an image.")]
        public int CreateOrUpdateImage(string ticket, TransitImage t_image)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Image image = t_image.GetImage(session);
                image.Modified = DateTime.UtcNow;
                session.SaveOrUpdate(image);
                session.Flush();
                return image.Id;
            }
        }

        [WebMethod(Description = "Create or update an image.")]
        public int CreateOrUpdateImageAttributes(string ticket, TransitImage t_image)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Image image = t_image.GetImage(session, false);
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
                DBlog.Data.Image image = (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id);
                return new TransitImage(session, image, ticket);
            }
        }

        [WebMethod(Description = "Delete an image.")]
        public void DeleteImage(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);

                Image image = session.Load<Image>(id);

                // delete comments associated with the image
                foreach (ImageComment instance in image.ImageComments)
                {
                    session.Delete(instance);
                    session.Delete(instance.Comment);
                }

                session.Delete(string.Format("FROM ImageCounter WHERE Image_Id = {0}", id));
                session.Delete(string.Format("FROM PostImage WHERE Image_Id = {0}", id));
                session.Delete(image);
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
                return query.Execute<int>();
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

                IList<Image> list = cr.List<Image>();

                List<TransitImage> result = new List<TransitImage>(list.Count);

                foreach (Image obj in list)
                {
                    result.Add(new TransitImage(session, obj, ticket));
                }

                return result;
            }
        }

        [WebMethod(Description = "Increment an image counter.")]
        public long IncrementImageCounter(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                long result = TransitCounter.Increment<Image, ImageCounter>(session, id);
                session.Flush();
                return result;
            }
        }

        [WebMethod(Description = "Increment image counters.")]
        public long IncrementImageCounters(string ticket, int[] ids)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                long result = 0;
                if (ids != null)
                {
                    foreach(int id in ids)
                    {
                        result += TransitCounter.Increment<Image, ImageCounter>(
                            session, id);
                    }
                }
                session.Flush();
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
                DBlog.Data.Image image = (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id);
                return new TransitImage(session, image, false, true, ticket);
            }
        }

        [WebMethod(Description = "Get image data.")]
        public EXIFMetaData GetImageEXIFMetaDataById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                DBlog.Data.Image image = (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id);
                TransitImage t_image = new TransitImage(session, image, false, true, ticket);
                EXIFMetaData exif_metadata = (t_image.Data != null) ? new EXIFMetaData(
                    new System.Drawing.Bitmap(new MemoryStream(t_image.Data)).PropertyItems) : null;
                return exif_metadata;
            }
        }


        [WebMethod(Description = "Get image data if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithBitmapByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                DBlog.Data.Image image = (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id);
                TransitImage img = new TransitImage(session, image, ticket);

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage(session, image, false, true, ticket);
            }
        }

        [WebMethod(Description = "Get image thumbnail.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                DBlog.Data.Image image = (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id);
                return new TransitImage(session, image, true, false, ticket); 
            }
        }

        [WebMethod(Description = "Get image thumbnail if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            // todo: check permissions with ticket
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                DBlog.Data.Image image = (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id);
                TransitImage img = new TransitImage(session, image, ticket);

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage(session, image, true, false, ticket);
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
                Reference reference = session.CreateCriteria(typeof(Reference))
                    .Add(Expression.Eq("Word", word))
                    .UniqueResult<Reference>();
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
        public int GetReferencesCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Reference), "Reference");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
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

                IList<Reference> list = cr.List<Reference>();

                List<TransitReference> result = new List<TransitReference>(list.Count);

                foreach (Reference obj in list)
                {
                    result.Add(new TransitReference(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Search references.")]
        public List<TransitReference> SearchReferences(string ticket, TransitReferenceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Reference));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<Reference> list = cr.List<Reference>();

                List<TransitReference> result = new List<TransitReference>(list.Count);

                foreach (Reference obj in list)
                {
                    result.Add(new TransitReference(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Search references count.")]
        public int SearchReferencesCount(string ticket, TransitReferenceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Reference), "Reference");
                if (options != null)
                {
                    options.Apply(query);
                }
                return query.Execute<int>();
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
                ManagedReferrerHostRollup mrhr = new ManagedReferrerHostRollup(referrerhostrollup);
                mrhr.RollupExistingReferrerHosts(session);
                return referrerhostrollup.Id;
            }
        }

        [WebMethod(Description = "Get referrer host rollups count.")]
        public int GetReferrerHostRollupsCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.ReferrerHostRollup), "ReferrerHostRollup");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
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

                IList<ReferrerHostRollup> list = cr.List <ReferrerHostRollup>();

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

        #region Referrer Search Queries

        [WebMethod(Description = "Get a referrer search query.")]
        public TransitReferrerSearchQuery GetReferrerSearchQueryById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitReferrerSearchQuery((ReferrerSearchQuery)session.Load(typeof(ReferrerSearchQuery), id));
            }
        }

        [WebMethod(Description = "Create or update a referrer search query.")]
        public int CreateOrUpdateReferrerSearchQuery(string ticket, TransitReferrerSearchQuery t_referrersearchquery)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                ReferrerSearchQuery referrersearchquery = t_referrersearchquery.GetReferrerSearchQuery(session);
                session.SaveOrUpdate(referrersearchquery);
                session.Flush();
                return referrersearchquery.Id;
            }
        }

        [WebMethod(Description = "Get referrer search queries count.")]
        public int GetReferrerSearchQueriesCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.ReferrerSearchQuery), "ReferrerSearchQuery").Execute<int>();
            }
        }

        [WebMethod(Description = "Get referrer search queries.")]
        public List<TransitReferrerSearchQuery> GetReferrerSearchQueries(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(ReferrerSearchQuery))
                    .AddOrder(Order.Desc("RequestCount"));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<ReferrerSearchQuery> list = cr.List<ReferrerSearchQuery>();

                List<TransitReferrerSearchQuery> result = new List<TransitReferrerSearchQuery>(list.Count);

                foreach (ReferrerSearchQuery obj in list)
                {
                    result.Add(new TransitReferrerSearchQuery(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a referrer search query.")]
        public void DeleteReferrerSearchQuery(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((ReferrerSearchQuery)session.Load(typeof(ReferrerSearchQuery), id));
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
        public int GetHighlightsCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Highlight), "Highlight");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
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

                IList<Highlight> list = cr.List<Highlight>();

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

        #region Posts

        [WebMethod(Description = "Get a post.")]
        public TransitPost GetPostById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                Post post = (Post)session.Load(typeof(Post), id);
                TransitPost t_post = new TransitPost(session, post, ticket);
                return t_post;
            }
        }

        [WebMethod(Description = "Check whether access is granted to post.")]
        public bool HasAccessToPost(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                Post post = (Post)session.Load(typeof(Post), id);
                return TransitPost.GetAccess(session, post, ticket);
            }
        }

        [WebMethod(Description = "Create or update a post.")]
        public int CreateOrUpdatePost(string ticket, TransitPost t_post)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                if (t_post.LoginId == 0) t_post.LoginId = ManagedLogin.GetLoginId(ticket);
                Post post = t_post.GetPost(session);
                post.Modified = DateTime.UtcNow;
                if (post.Id == 0) post.Created = post.Modified;
                session.SaveOrUpdate(post);
                List<PostTopic> postTopicsToBeCreated = null;
                List<PostTopic> postTopicsToBeDeleted = null;
                TransitTopic.MergeTo(session, post, t_post.Topics, out postTopicsToBeCreated, out postTopicsToBeDeleted);
                foreach (PostTopic postTopic in postTopicsToBeCreated) session.Save(postTopic);
                foreach (PostTopic postTopic in postTopicsToBeDeleted) session.Delete(postTopic);
                session.Flush();
                return post.Id;
            }
        }

        [WebMethod(Description = "Get posts count.")]
        public int GetPostsCount(string ticket, TransitPostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                StringCriteria criteria = new StringCriteria(session, "Post", typeof(Post));

                if (options != null)
                {
                    options.Apply(criteria);
                }

                IQuery sqlquery = criteria.CreateQuery();

                return (int)sqlquery.List().Count;
            }
        }

        [WebMethod(Description = "Get posts.")]
        public List<TransitPost> GetPosts(string ticket, TransitPostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                StringCriteria criteria = new StringCriteria(session, "Post", typeof(Post));
                criteria.AddOrder("Sticky", WebServiceQuerySortDirection.Descending);

                if (options != null)
                {
                    options.Apply(criteria);
                }

                IQuery sqlquery = criteria.CreateQuery();

                if (options != null)
                {
                    options.Apply(sqlquery);
                }

                IList list = sqlquery.List();

                List<TransitPost> result = new List<TransitPost>(list.Count);

                foreach (Post obj in list)
                {
                    result.Add(new TransitPost(session, obj, ticket));
                }

                return result;
            }
        }

        [WebMethod(Description = "Get posts count with joined tables.")]
        public int GetPostsCountEx(string ticket, TransitPostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                string[] tables = { "PostCounter", "Counter" };
                StringCriteria criteria = new StringCriteria(session, "Post", typeof(Post), tables);
                criteria.Add("{Post}.Post_Id = PostCounter.Post_Id");
                criteria.Add("Counter.Counter_Id = PostCounter.Counter_Id");

                if (options != null)
                {
                    options.Apply(criteria);
                }

                IQuery query = criteria.CreateQuery();

                if (options != null)
                {
                    options.Apply(query);
                }

                return (int) query.List().Count;
            }
        }

        [WebMethod(Description = "Get posts with joined tables.")]
        public List<TransitPost> GetPostsEx(string ticket, TransitPostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                string[] tables = { "PostCounter", "Counter" };
                StringCriteria criteria = new StringCriteria(session, "Post", typeof(Post), tables);
                criteria.Add("{Post}.Post_Id = PostCounter.Post_Id");
                criteria.Add("Counter.Counter_Id = PostCounter.Counter_Id");

                if (options != null)
                {
                    options.Apply(criteria);
                }

                criteria.AddOrder("Post.Sticky", WebServiceQuerySortDirection.Descending);

                IQuery query = criteria.CreateQuery();

                if (options != null)
                {
                    options.Apply(query);
                }

                IList list = query.List();

                List<TransitPost> result = new List<TransitPost>(list.Count);

                foreach (Post obj in list)
                {
                    result.Add(new TransitPost(session, obj, ticket));
                }

                return result;
            }
        }


        [WebMethod(Description = "Delete a post.")]
        public void DeletePost(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Post post = session.Load<Post>(id);

                // delete images associated with the post
                foreach (PostImage instance in post.PostImages)
                {
                    session.Delete(instance);
                    session.Delete(instance.Image);
                }

                // delete comments associated with the post
                foreach (PostComment instance in post.PostComments)
                {
                    session.Delete(instance);
                    session.Delete(instance.Comment);
                }

                session.Delete(string.Format("FROM PostLogin WHERE Post_Id = {0}", id));
                session.Delete(string.Format("FROM PostCounter WHERE Post_Id = {0}", id));
                session.Delete(post);
                session.Flush();
            }
        }

        [WebMethod(Description = "Increment a post counter.")]
        public long IncrementPostCounter(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                long result = TransitCounter.Increment<Post, PostCounter>(session, id);
                session.Flush();
                return result;
            }
        }

        [WebMethod(Description = "Increment post counters.")]
        public long IncrementPostCounters(string ticket, int[] ids)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                long result = 0;
                if (ids != null)
                {
                    foreach(int id in ids)
                    {
                        result += TransitCounter.Increment<Post, PostCounter>(
                            session, id);
                    }
                }
                session.Flush();
                return result;
            }
        }


        [WebMethod(Description = "Update statistics for a series of requests.")]
        public int CreateOrUpdateStats(string ticket, TransitBrowser[] t_browsers, TransitReferrerHost[] t_rhs, TransitReferrerSearchQuery[] t_rsqs)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                int increment = 0;

                if (t_browsers != null)
                {
                    if (t_browsers.Length > increment) increment = t_browsers.Length;
                    foreach (TransitBrowser t_browser in t_browsers)
                    {
                        Browser browser = t_browser.GetBrowser(session);
                        session.SaveOrUpdate(browser);
                    }
                }

                if (t_rhs != null)
                {
                    if (t_rhs.Length > increment) increment = t_rhs.Length;
                    foreach (TransitReferrerHost t_rh in t_rhs)
                    {
                        ReferrerHost rh = t_rh.GetReferrerHost(session);
                        session.SaveOrUpdate(rh);
                    }
                }

                if (t_rsqs != null)
                {
                    if (t_rsqs.Length > increment) increment = t_rsqs.Length;
                    foreach (TransitReferrerSearchQuery t_rsq in t_rsqs)
                    {
                        ReferrerSearchQuery rsq = t_rsq.GetReferrerSearchQuery(session);
                        session.SaveOrUpdate(rsq);
                    }
                }

                TransitCounter.IncrementCounters(session, increment);

                session.Flush();
                return Math.Max(t_browsers.Length, t_rhs.Length);
            }
        }

        [WebMethod(Description = "Get stats summary.")]
        public TransitStats GetStats(string ticket, int reserved)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitStats(session);
            }            
        }

        [WebMethod(Description = "Get stats summary counters.")]
        public List<TransitCounter> GetStatsSummary(string ticket, TransitStatsQueryOptions options)
        {

            if (options == null)
            {
                throw new ArgumentException("Missing Options");
            }

            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                switch (options.Type)
                {
                    case TransitStats.Type.Daily:
                        return TransitStats.GetSummaryDaily(session);
                    case TransitStats.Type.Hourly:
                        return TransitStats.GetSummaryHourly(session);
                    case TransitStats.Type.Monthly:
                        return TransitStats.GetSummaryMonthly(session);
                    case TransitStats.Type.Weekly:
                        return TransitStats.GetSummaryWeekly(session);
                    case TransitStats.Type.Yearly:
                        return TransitStats.GetSummaryYearly(session);
                }

                throw new ArgumentException("Invalid Summary Type");
            }
        }

        #endregion

        #region Post Images

        [WebMethod(Description = "Create or update a post image.")]
        public int CreateOrUpdatePostImage(string ticket, int post_id, TransitImage t_image)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);

                Post post = (Post)session.Load(typeof(Post), post_id);

                Image image = t_image.GetImage(session);
                image.Modified = DateTime.UtcNow;
                session.SaveOrUpdate(image);

                PostImage post_image = session.CreateCriteria(typeof(PostImage))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Image.Id", t_image.Id))
                    .UniqueResult<PostImage>();

                if (post_image == null)
                {
                    post_image = new PostImage();
                    post_image.Post = post;
                    post_image.Image = image;
                    session.SaveOrUpdate(post_image);
                }

                session.Flush();
                return image.Id;
            }
        }

        [WebMethod(Description = "Get a post image.")]
        public TransitPostImage GetPostImageById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                PostImage pi = (PostImage) session.Load(typeof(DBlog.Data.PostImage), id);
                return new TransitPostImage(session, pi, ticket);
            }
        }

        [WebMethod(Description = "Delete a post image.")]
        public void DeletePostImage(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                PostImage ei = (PostImage)session.Load(typeof(PostImage), id);
                session.Delete(ei);
                session.Delete(ei.Image);
                session.Flush();
            }
        }

        [WebMethod(Description = "Get post image count.")]
        public int GetPostImagesCount(string ticket, TransitPostImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.PostImage), "PostImage");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get post images.")]
        public List<TransitPostImage> GetPostImages(string ticket, TransitPostImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(PostImage));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<PostImage> list = cr.List<PostImage>();

                List<TransitPostImage> result = new List<TransitPostImage>(list.Count);

                int index = (options != null) ? options.FirstResult : 0;
                foreach (PostImage obj in list)
                {
                    TransitPostImage tpi = new TransitPostImage(session, obj, ticket);
                    tpi.Index = index;
                    index++;
                    result.Add(tpi);
                }

                return result;
            }
        }

        [WebMethod(Description = "Get post images count with joined tables.")]
        public int GetPostImagesCountEx(string ticket, TransitPostImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                StringBuilder q = new StringBuilder();
                q.AppendLine("SELECT COUNT(i) FROM Post p, PostImage pi, Image i");
                if (options.Counters) q.AppendLine(", ImageCounter ic, Counter c");
                q.AppendLine("WHERE p.Id = pi.Post.Id AND pi.Image.Id = i.Id");
                if (options.Counters) q.AppendLine("AND i.Id = ic.Image.Id AND ic.Counter.Id = c.Id");

                if (options != null && options.PostId > 0)
                {
                    q.AppendLine(string.Format("AND p.Id = {0}", options.PostId));
                }

                if (options != null && options.PreferredOnly)
                {
                    q.AppendLine("AND i.Preferred = 1");
                }

                IQuery query = session.CreateQuery(q.ToString());

                if (options != null)
                {
                    options.Apply(query);
                }

                return (int) query.UniqueResult<long>();
            }
        }


        [WebMethod(Description = "Get post images with joined tables.")]
        public List<TransitPostImage> GetPostImagesEx(string ticket, TransitPostImageQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                StringBuilder q = new StringBuilder();

                q.AppendLine("SELECT {PostImage.*} FROM Post, PostImage {PostImage}, Image");
                if (options.Counters) q.AppendLine(", ImageCounter, Counter");
                q.AppendLine("WHERE Post.Post_Id = {PostImage}.Post_Id AND {PostImage}.Image_Id = Image.Image_Id");
                if (options.Counters) q.AppendLine("AND Image.Image_Id = ImageCounter.Image_Id AND ImageCounter.Counter_Id = Counter.Counter_Id");

                if (options != null && options.PostId > 0)
                {
                    q.AppendLine(string.Format("AND Post.Post_Id = {0}", options.PostId));
                }

                if (options != null && options.PreferredOnly)
                {
                    q.AppendLine("AND Image.Preferred = 1");
                }

                if (options != null && !string.IsNullOrEmpty(options.SortExpression))
                {
                    q.AppendLine(string.Format("ORDER BY {0} {1}",
                        Renderer.SqlEncode(options.SortExpression),
                        options.SortDirection == WebServiceQuerySortDirection.Ascending ? string.Empty : "DESC"));
                }

                IQuery query = session.CreateSQLQuery(q.ToString(), "PostImage", typeof(PostImage));

                if (options != null)
                {
                    options.Apply(query);
                }

                IList list = query.List();

                List<TransitPostImage> result = new List<TransitPostImage>(list.Count);
                int index = (options != null) ? options.FirstResult : 0;

                foreach (PostImage obj in list)
                {
                    TransitPostImage tpi = new TransitPostImage(session, obj, ticket);
                    tpi.Index = index;
                    index++;
                    result.Add(tpi);
                }

                return result;
            }
        }

        #endregion

        #region Permalinks

        [WebMethod(Description = "Get a permalink.")]
        public TransitPermalink GetPermalinkById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitPermalink((Permalink)session.Load(typeof(Permalink), id));
            }
        }

        [WebMethod(Description = "Get a permalink by source id and type.")]
        public TransitPermalink GetPermalinkBySource(string ticket, TransitPermalinkQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ICriteria cr = session.CreateCriteria(typeof(Permalink));

                if (options != null)
                {
                    options.Apply(cr);
                }

                return new TransitPermalink(cr.UniqueResult<Permalink>());
            }
        }

        [WebMethod(Description = "Create or update a permalink.")]
        public int CreateOrUpdatePermalink(string ticket, TransitPermalink t_permalink)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Permalink permalink = t_permalink.GetPermalink(session);
                session.SaveOrUpdate(permalink);
                session.Flush();
                return permalink.Id;
            }
        }

        [WebMethod(Description = "Get permalinks count.")]
        public int GetPermalinksCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Permalink), "Permalink");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get permalinks.")]
        public List<TransitPermalink> GetPermalinks(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Permalink));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<Permalink> list = cr.List<Permalink>();

                List<TransitPermalink> result = new List<TransitPermalink>(list.Count);

                foreach (Permalink obj in list)
                {
                    result.Add(new TransitPermalink(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a permalink.")]
        public void DeletePermalink(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Permalink)session.Load(typeof(Permalink), id));
                session.Flush();
            }
        }

        #endregion

        #region Comments

        [WebMethod(Description = "Get a comment.")]
        public TransitComment GetCommentById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                Comment c = (Comment)session.Load(typeof(Comment), id);
                return new TransitComment(session, c, ticket);
            }
        }

        [WebMethod(Description = "Create or update a comment.")]
        public int CreateOrUpdateComment(string ticket, TransitComment t_comment)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Comment comment = t_comment.GetComment(session);
                session.SaveOrUpdate(comment);
                session.Flush();
                return comment.Id;
            }
        }

        [WebMethod(Description = "Get comments count.")]
        public int GetCommentsCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Comment), "Comment");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get comments.")]
        public List<TransitComment> GetComments(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Comment));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<Comment> list = cr.List<Comment>();

                List<TransitComment> result = new List<TransitComment>(list.Count);

                foreach (Comment obj in list)
                {
                    result.Add(new TransitComment(session, obj, ticket));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a comment.")]
        public void DeleteComment(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Comment)session.Load(typeof(Comment), id));
                session.Flush();
            }
        }

        #endregion

        #region Post Comments

        [WebMethod(Description = "Create or update a post comment.")]
        public int CreateOrUpdatePostComment(string ticket, int post_id, TransitComment t_comment)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                Post post = (Post)session.Load(typeof(Post), post_id);

                if (string.IsNullOrEmpty(ticket))
                {
                    // anonymous
                    t_comment.LoginId = 0;
                }
                else if (t_comment.LoginId == 0)
                {
                    // logged in
                    t_comment.LoginId = ManagedLogin.GetLoginId(ticket);
                }
                else if (t_comment.LoginId != ManagedLogin.GetLoginId(ticket) && ! ManagedLogin.IsAdministrator(session, ticket))
                {    
                    // not admin and trying to post a comment as someone else
                    throw new ManagedLogin.AccessDeniedException();
                }

                Comment comment = t_comment.GetComment(session);
                comment.Modified = DateTime.UtcNow;
                if (comment.Id == 0) comment.Created = comment.Modified;
                session.SaveOrUpdate(comment);

                if (t_comment.ParentCommentId != 0 && t_comment.Id == 0)
                {
                    Thread thread = new Thread();
                    thread.Comment = comment;
                    thread.ParentComment = (Comment) session.Load(typeof(Comment), t_comment.ParentCommentId);
                    session.Save(thread);
                }

                PostComment post_comment = session.CreateCriteria(typeof(PostComment))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Comment.Id", t_comment.Id))
                    .UniqueResult<PostComment>();

                if (post_comment == null)
                {
                    post_comment = new PostComment();
                    post_comment.Post = post;
                    post_comment.Comment = comment;
                    session.SaveOrUpdate(post_comment);
                }

                ManagedPostComment.Notify(post_comment);

                session.Flush();
                return comment.Id;
            }
        }

        [WebMethod(Description = "Get a post comment.")]
        public TransitPostComment GetPostCommentById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                PostComment pc = (PostComment) session.Load(typeof(DBlog.Data.PostComment), id);
                return new TransitPostComment(session, pc, ticket);
            }
        }

        [WebMethod(Description = "Delete a post comment.")]
        public void DeletePostComment(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                PostComment ei = (PostComment)session.Load(typeof(PostComment), id);
                session.Delete(ei);
                session.Delete(ei.Comment);
                session.Flush();
            }
        }

        [WebMethod(Description = "Get post comment count.")]
        public int GetPostCommentsCount(string ticket, TransitPostCommentQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.PostComment), "PostComment");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get all associated comments.")]
        public List<TransitAssociatedComment> GetAssociatedComments(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                IQuery sqlquery = session.GetNamedQuery("GetAssociatedComments");

                //if (options != null)
                //{
                //    options.Apply(sqlquery);
                //}

                IList<AssociatedComment> list = sqlquery.List<AssociatedComment>();

                List<TransitAssociatedComment> result = new List<TransitAssociatedComment>(list.Count);
                if (options == null) options = new WebServiceQueryOptions(list.Count, 0);
                for (int i = 0; i < options.PageSize; i++)
                {
                    int index = options.FirstResult + i;
                    if (index >= list.Count) 
                        break;
                    
                    result.Add(new TransitAssociatedComment(session, list[index], ticket));
                }

                return result;
            }
        }

        [WebMethod(Description = "Get all associated comments count.")]
        public int GetAssociatedCommentsCount(string ticket, WebServiceQueryOptions options)
        {
            return GetCommentsCount(ticket, options);
        }

        [WebMethod(Description = "Get post comments.")]
        public List<TransitPostComment> GetPostComments(string ticket, TransitPostCommentQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(PostComment));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<PostComment> list = cr.List<PostComment>();

                List<TransitPostComment> result = new List<TransitPostComment>(list.Count);

                foreach (PostComment obj in list)
                {
                    if (obj.Comment.Threads == null || obj.Comment.Threads.Count == 0)
                    {
                        result.Insert(0, new TransitPostComment(session, obj, ticket));
                    }
                    else
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].CommentId == ((Thread)obj.Comment.Threads[0]).ParentComment.Id)
                            {
                                result.Insert(i + 1, new TransitPostComment(session, obj, ticket));
                                break;
                            }
                        }
                    }
                }

                return result;
            }
        }
        #endregion

        #region Image Comments

        [WebMethod(Description = "Create or update a image comment.")]
        public int CreateOrUpdateImageComment(string ticket, int image_id, TransitComment t_comment)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                Image image = (Image)session.Load(typeof(Image), image_id);

                if (string.IsNullOrEmpty(ticket))
                {
                    // anonymous
                    t_comment.LoginId = 0;
                }
                else if (t_comment.LoginId == 0)
                {
                    // logged in
                    t_comment.LoginId = ManagedLogin.GetLoginId(ticket);
                }
                else if (t_comment.LoginId != ManagedLogin.GetLoginId(ticket) && !ManagedLogin.IsAdministrator(session, ticket))
                {
                    // not admin and trying to image a comment as someone else
                    throw new ManagedLogin.AccessDeniedException();
                }

                Comment comment = t_comment.GetComment(session);
                comment.Modified = DateTime.UtcNow;
                if (comment.Id == 0) comment.Created = comment.Modified;
                session.SaveOrUpdate(comment);

                if (t_comment.ParentCommentId != 0 && t_comment.Id == 0)
                {
                    Thread thread = new Thread();
                    thread.Comment = comment;
                    thread.ParentComment = (Comment)session.Load(typeof(Comment), t_comment.ParentCommentId);
                    session.Save(thread);
                }

                ImageComment image_comment = session.CreateCriteria(typeof(ImageComment))
                    .Add(Expression.Eq("Image.Id", image_id))
                    .Add(Expression.Eq("Comment.Id", t_comment.Id))
                    .UniqueResult<ImageComment>();

                if (image_comment == null)
                {
                    image_comment = new ImageComment();
                    image_comment.Image = image;
                    image_comment.Comment = comment;
                    session.SaveOrUpdate(image_comment);
                }

                session.Flush();
                return comment.Id;
            }
        }

        [WebMethod(Description = "Get a image comment.")]
        public TransitImageComment GetImageCommentById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                DBlog.Data.ImageComment ic = (ImageComment) session.Load(typeof(DBlog.Data.ImageComment), id);
                return new TransitImageComment(session, ic, ticket);
            }
        }

        [WebMethod(Description = "Delete a image comment.")]
        public void DeleteImageComment(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                ImageComment ei = (ImageComment)session.Load(typeof(ImageComment), id);
                session.Delete(ei);
                session.Delete(ei.Comment);
                session.Flush();
            }
        }

        [WebMethod(Description = "Get image comment count.")]
        public int GetImageCommentsCount(string ticket, TransitImageCommentQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.ImageComment), "ImageComment");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get image comments.")]
        public List<TransitImageComment> GetImageComments(string ticket, TransitImageCommentQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(ImageComment));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<ImageComment> list = cr.List<ImageComment>();

                List<TransitImageComment> result = new List<TransitImageComment>(list.Count);

                foreach (ImageComment obj in list)
                {
                    if (obj.Comment.Threads == null || obj.Comment.Threads.Count == 0)
                    {
                        result.Insert(0, new TransitImageComment(session, obj, ticket));
                    }
                    else
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].CommentId == ((Thread)obj.Comment.Threads[0]).ParentComment.Id)
                            {
                                result.Insert(i + 1, new TransitImageComment(session, obj, ticket));
                                break;
                            }
                        }
                    }
                }

                return result;
            }
        }
        #endregion

        #region Counters

        [WebMethod(Description = "Increment counters.")]
        public void IncrementCounters(string ticket, int count)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitCounter.IncrementCounters(session, count);
                session.Flush();
            }
        }

        [WebMethod(Description = "Increment named counter.")]
        public void IncrementNamedCounter(string ticket, string name, int count)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ITransaction t = session.BeginTransaction();
                try
                {
                    NamedCounter counter = session.CreateCriteria(typeof(NamedCounter))
                        .Add(Expression.Eq("Name", name))
                        .UniqueResult<NamedCounter>();

                    if (counter == null)
                    {
                        counter = new NamedCounter();
                        counter.Name = name;
                        counter.Counter = new Counter();
                        counter.Counter.Count = 1;
                        counter.Counter.Created = DateTime.UtcNow;
                        session.Save(counter.Counter);
                        session.Save(counter);
                    }
                    else
                    {
                        counter.Counter.Count += count;
                        session.Save(counter.Counter);
                    }

                    session.Flush();
                    t.Commit();
                }
                catch
                {
                    t.Rollback();
                }
            }
        }

        [WebMethod(Description = "Get the lifetime hourly count sum.")]
        public TransitCounter GetHourlyCountSum(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                
                TransitCounter tc = new TransitCounter();

                object[] result = (object[]) session.CreateQuery(
                    "SELECT SUM(hc.RequestCount), MIN(hc.DateTime) FROM HourlyCounter hc")
                    .UniqueResult();

                if (result != null)
                {
                    tc.Count = (result[0] == null) ? 0 : (long) result[0];
                    tc.Created = (result[1] == null) ? DateTime.UtcNow : (DateTime) result[1];
                }

                return tc;
            }
        }

        #endregion

        #region Feeds

        [WebMethod(Description = "Get a feed.")]
        public TransitFeed GetFeedById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitFeed((Feed)session.Load(typeof(Feed), id));
            }
        }

        [WebMethod(Description = "Create or update a feed.")]
        public int CreateOrUpdateFeed(string ticket, TransitFeed t_feed)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Feed feed = t_feed.GetFeed(session);
                if (t_feed.Id == 0)
                {
                    // HACK: these are nullable and should be NULL
                    feed.Updated = new DateTime(1900, 1, 1);
                    feed.Saved = new DateTime(1900, 1, 1);
                }
                session.SaveOrUpdate(feed);
                session.Flush();
                return feed.Id;
            }
        }

        [WebMethod(Description = "Get feeds count.")]
        public int GetFeedsCount(string ticket, TransitFeedQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery q = new CountQuery(session, typeof(DBlog.Data.Feed), "Feed");
                if (options != null)
                {
                    options.Apply(q);
                }
                return q.Execute<int>();
            }
        }

        [WebMethod(Description = "Get feeds.")]
        public List<TransitFeed> GetFeeds(string ticket, TransitFeedQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ICriteria cr = session.CreateCriteria(typeof(Feed));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<Feed> list = cr.List<Feed>();

                List<TransitFeed> result = new List<TransitFeed>(list.Count);

                bool fAdmin = ManagedLogin.IsAdministrator(session, ticket);

                foreach (Feed obj in list)
                {
                    if (!fAdmin)
                    {
                        obj.Username = string.Empty;
                        obj.Password = string.Empty;
                    }

                    result.Add(new TransitFeed(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a feed.")]
        public void DeleteFeed(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Feed)session.Load(typeof(Feed), id));
                session.Flush();
            }
        }

        [WebMethod(Description = "Update a feed.")]
        public int UpdateFeed(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                int result = ManagedFeed.Update((Feed) session.Load(typeof(Feed), id), session, false);
                session.Flush();
                return result;
            }
        }


        #endregion

        #region Feed Items

        [WebMethod(Description = "Get a feed item.")]
        public TransitFeedItem GetFeedItemById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitFeedItem((FeedItem)session.Load(typeof(FeedItem), id));
            }
        }

        [WebMethod(Description = "Create or update a feed item.")]
        public int CreateOrUpdateFeedItem(string ticket, TransitFeedItem t_feeditem)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                FeedItem feeditem = t_feeditem.GetFeedItem(session);
                session.SaveOrUpdate(feeditem);
                session.Flush();
                return feeditem.Id;
            }
        }

        [WebMethod(Description = "Get feed items count.")]
        public int GetFeedItemsCount(string ticket, TransitFeedItemQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery q = new CountQuery(session, typeof(DBlog.Data.FeedItem), "FeedItem");
                if (options != null)
                {
                    options.Apply(q);
                }
                return q.Execute<int>();
            }
        }

        [WebMethod(Description = "Get feed items.")]
        public List<TransitFeedItem> GetFeedItems(string ticket, TransitFeedItemQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(FeedItem));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<FeedItem> list = cr.List<FeedItem>();

                List<TransitFeedItem> result = new List<TransitFeedItem>(list.Count);

                foreach (FeedItem obj in list)
                {
                    result.Add(new TransitFeedItem(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a feed item.")]
        public void DeleteFeedItem(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((FeedItem)session.Load(typeof(FeedItem), id));
                session.Flush();
            }
        }

        #endregion

        #region Post Logins

        [WebMethod(Description = "Create or update a post login.")]
        public int CreateOrUpdatePostLogin(string ticket, int post_id, TransitLogin t_login)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);

                Post post = (Post)session.Load(typeof(Post), post_id);

                Login login = t_login.GetLogin(session);
                session.SaveOrUpdate(login);

                PostLogin post_login = session.CreateCriteria(typeof(PostLogin))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Login.Id", t_login.Id))
                    .UniqueResult<PostLogin>();

                if (post_login == null)
                {
                    post_login = new PostLogin();
                    post_login.Post = post;
                    post_login.Login = login;
                    session.SaveOrUpdate(post_login);
                }

                session.Flush();
                return login.Id;
            }
        }

        [WebMethod(Description = "Get a post login.")]
        public TransitPostLogin GetPostLoginById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitPostLogin(session, (PostLogin)session.Load(typeof(DBlog.Data.PostLogin), id));
            }
        }

        [WebMethod(Description = "Delete a post login.")]
        public void DeletePostLogin(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                PostLogin ei = (PostLogin)session.Load(typeof(PostLogin), id);
                session.Delete(ei);
                session.Flush();
            }
        }

        [WebMethod(Description = "Get post login count.")]
        public int GetPostLoginsCount(string ticket, TransitPostLoginQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.PostLogin), "PostLogin");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get post logins.")]
        public List<TransitPostLogin> GetPostLogins(string ticket, TransitPostLoginQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(PostLogin));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<PostLogin> list = cr.List<PostLogin>();

                List<TransitPostLogin> result = new List<TransitPostLogin>(list.Count);

                foreach (PostLogin obj in list)
                {
                    result.Add(new TransitPostLogin(session, obj));
                }

                return result;
            }
        }

        #endregion

        #region Browsers

        [WebMethod(Description = "Get a browser.")]
        public TransitBrowser GetBrowserById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitBrowser((Browser)session.Load(typeof(Browser), id));
            }
        }

        [WebMethod(Description = "Create or update a browser.")]
        public int CreateOrUpdateBrowser(string ticket, TransitBrowser t_browser)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                Browser browser = t_browser.GetBrowser(session);
                session.SaveOrUpdate(browser);
                session.Flush();
                return browser.Id;
            }
        }

        [WebMethod(Description = "Increment a browser counter.")]
        public long IncrementBrowserCounter(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                long result = TransitCounter.Increment<Browser, BrowserCounter>(session, id);
                session.Flush();
                return result;
            }
        }

        [WebMethod(Description = "Get browsers count.")]
        public int GetBrowsersCount(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CountQuery query = new CountQuery(session, typeof(DBlog.Data.Browser), "Browser");
                if (options != null) options.Apply(query);
                return query.Execute<int>();
            }
        }

        [WebMethod(Description = "Get browsers.")]
        public List<TransitBrowser> GetBrowsers(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(Browser));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<Browser> list = cr.List<Browser>();

                List<TransitBrowser> result = new List<TransitBrowser>(list.Count);

                foreach (Browser obj in list)
                {
                    result.Add(new TransitBrowser(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a browser.")]
        public void DeleteBrowser(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((Browser)session.Load(typeof(Browser), id));
                session.Flush();
            }
        }

        #endregion

        #region Referrer Hosts

        [WebMethod(Description = "Get a referrer host.")]
        public TransitReferrerHost GetReferrerHostById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitReferrerHost((ReferrerHost)session.Load(typeof(ReferrerHost), id));
            }
        }

        [WebMethod(Description = "Create or update a referrer host.")]
        public int CreateOrUpdateReferrerHost(string ticket, TransitReferrerHost t_referrerhost)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                ReferrerHost referrerhost = t_referrerhost.GetReferrerHost(session);
                session.SaveOrUpdate(referrerhost);
                session.Flush();
                return referrerhost.Id;
            }
        }

        [WebMethod(Description = "Get referrer hosts count.")]
        public int GetReferrerHostsCount(string ticket, TransitReferrerHostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                StringCriteria criteria = new StringCriteria(session, "ReferrerHost", typeof(ReferrerHost));
                if (options != null)
                {
                    options.Apply(criteria);
                }
                IQuery sqlquery = criteria.CreateQuery();
                return (int)sqlquery.List().Count;
            }
        }

        [WebMethod(Description = "Get referrer hosts.")]
        public List<TransitReferrerHost> GetReferrerHosts(string ticket, TransitReferrerHostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(ReferrerHost))
                    .AddOrder(Order.Desc("RequestCount"));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList<ReferrerHost> list = cr.List<ReferrerHost>();

                List<TransitReferrerHost> result = new List<TransitReferrerHost>(list.Count);

                foreach (ReferrerHost obj in list)
                {
                    result.Add(new TransitReferrerHost(obj));
                }

                return result;
            }
        }

        [WebMethod(Description = "Delete a referrer host.")]
        public void DeleteReferrerHost(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                CheckAdministrator(session, ticket);
                session.Delete((ReferrerHost)session.Load(typeof(ReferrerHost), id));
                session.Flush();
            }
        }

        #endregion

    }
}