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
                Login login = (Login)session.CreateCriteria(typeof(Login))
                    .Add(Expression.Eq("Username", username))
                    .UniqueResult();

                if (login == null)
                {
                    throw new Exception("User Not Found");
                }

                return new TransitLogin(login);
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
                return TransitPost.HasAccess(session, post, ticket);
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
                CountQuery q = new CountQuery(session, typeof(DBlog.Data.Post), "Post");

                if (options != null)
                {
                    options.Apply(q);
                }

                return q.Execute();
            }
        }

        [WebMethod(Description = "Get posts.")]
        public List<TransitPost> GetPosts(string ticket, TransitPostQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                ICriteria cr = session.CreateCriteria(typeof(Post))
                    .AddOrder(Order.Desc("Modified"));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

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
                Post post = (Post)session.Load(typeof(Post), id);

                foreach (PostImage ei in post.PostImages)
                {
                    session.Delete(ei);
                    session.Delete(ei.Image);
                }

                session.Delete(string.Format("FROM PostLogin WHERE Post_Id = {0}", id));
                session.Delete(string.Format("FROM PostComment WHERE Post_Id = {0}", id));
                session.Delete(string.Format("FROM PostCounter WHERE Post_Id = {0}", id));
                session.Delete(post);
                session.Flush();
            }
        }

        [WebMethod(Description = "Increment an image counter.")]
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

        [WebMethod(Description = "Update statistics for a series of requests.")]
        public int CreateOrUpdateStats(string ticket, TransitBrowser[] t_browsers, TransitReferrerHost[] t_rhs)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                TransitCounter.IncrementCounters(session, Math.Max(t_browsers.Length, t_rhs.Length));

                if (t_browsers != null)
                {
                    foreach (TransitBrowser t_browser in t_browsers)
                    {
                        Browser browser = t_browser.GetBrowser(session);
                        session.SaveOrUpdate(browser);
                    }
                }

                if (t_rhs != null)
                {
                    foreach (TransitReferrerHost t_rh in t_rhs)
                    {
                        ReferrerHost rh = t_rh.GetReferrerHost(session);
                        session.SaveOrUpdate(rh);
                    }
                }

                session.Flush();
                return Math.Max(t_browsers.Length, t_rhs.Length);
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

                PostImage post_image = (PostImage)session.CreateCriteria(typeof(PostImage))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Image.Id", t_image.Id))
                    .UniqueResult();

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
                return query.Execute();
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

                IList list = cr.List();

                List<TransitPostImage> result = new List<TransitPostImage>(list.Count);

                foreach (PostImage obj in list)
                {
                    result.Add(new TransitPostImage(session, obj, ticket));
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
        public TransitPermalink GetPermalinkBySource(string ticket, int source_id, string source_type)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitPermalink(
                    (Permalink) session.CreateCriteria(typeof(Permalink))
                        .Add(Expression.Eq("SourceId", source_id))
                        .Add(Expression.Eq("SourceType", source_type))
                        .UniqueResult());
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
        public int GetPermalinksCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Permalink), "Permalink").Execute();
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

                IList list = cr.List();

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
        public int GetCommentsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Comment), "Comment").Execute();
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

                IList list = cr.List();

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

                PostComment post_comment = (PostComment)session.CreateCriteria(typeof(PostComment))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Comment.Id", t_comment.Id))
                    .UniqueResult();

                if (post_comment == null)
                {
                    post_comment = new PostComment();
                    post_comment.Post = post;
                    post_comment.Comment = comment;
                    session.SaveOrUpdate(post_comment);
                }

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
                return query.Execute();
            }
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

                IList list = cr.List();

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

                ImageComment image_comment = (ImageComment)session.CreateCriteria(typeof(ImageComment))
                    .Add(Expression.Eq("Image.Id", image_id))
                    .Add(Expression.Eq("Comment.Id", t_comment.Id))
                    .UniqueResult();

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
                return query.Execute();
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

                IList list = cr.List();

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
                    NamedCounter counter = (NamedCounter) session.CreateCriteria(typeof(NamedCounter))
                        .Add(Expression.Eq("Name", name))
                        .UniqueResult();

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
                    tc.Count = (result[0] == null) ? 0 : (int) result[0];
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
                return q.Execute();
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

                IList list = cr.List();

                List<TransitFeed> result = new List<TransitFeed>(list.Count);

                foreach (Feed obj in list)
                {
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
                return q.Execute();
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

                IList list = cr.List();

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

                PostLogin post_login = (PostLogin)session.CreateCriteria(typeof(PostLogin))
                    .Add(Expression.Eq("Post.Id", post_id))
                    .Add(Expression.Eq("Login.Id", t_login.Id))
                    .UniqueResult();

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
                return query.Execute();
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

                IList list = cr.List();

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
        public int GetBrowsersCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.Browser), "Browser").Execute();
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

                IList list = cr.List();

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
        public int GetReferrerHostsCount(string ticket)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new CountQuery(session, typeof(DBlog.Data.ReferrerHost), "ReferrerHost").Execute();
            }
        }

        [WebMethod(Description = "Get referrer hosts.")]
        public List<TransitReferrerHost> GetReferrerHosts(string ticket, WebServiceQueryOptions options)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                ICriteria cr = session.CreateCriteria(typeof(ReferrerHost));

                if (options != null)
                {
                    options.Apply(cr);
                }

                IList list = cr.List();

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