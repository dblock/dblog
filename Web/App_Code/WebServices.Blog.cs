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
                return new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id));
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
                    result.Add(new TransitImage(session, obj));
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
                return new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), false, true);
            }
        }

        [WebMethod(Description = "Get image data if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithBitmapByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitImage img = new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id));

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), false, true);
            }
        }

        [WebMethod(Description = "Get image thumbnail.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailById(string ticket, int id)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                return new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), true, false); 
            }
        }

        [WebMethod(Description = "Get image thumbnail if modified since.", BufferResponse = true)]
        public TransitImage GetImageWithThumbnailByIdIfModifiedSince(string ticket, int id, DateTime ifModifiedSince)
        {
            // todo: check permissions with ticket
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;
                TransitImage img = new TransitImage(session, (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), id));

                if (img.Modified <= ifModifiedSince)
                {
                    return null;
                }

                return new TransitImage(session, (DBlog.Data.Image) session.Load(typeof(DBlog.Data.Image), id), true, false);
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
                return new TransitPost(session, (Post)session.Load(typeof(Post), id));
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
                    result.Add(new TransitPost(session, obj));
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
                return new TransitPostImage(session, (PostImage) session.Load(typeof(DBlog.Data.PostImage), id));
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
                    result.Add(new TransitPostImage(session, obj));
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
                return new TransitComment((Comment)session.Load(typeof(Comment), id));
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
                    result.Add(new TransitComment(obj));
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
                return new TransitPostComment(session, (PostComment)session.Load(typeof(DBlog.Data.PostComment), id));
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
                        result.Insert(0, new TransitPostComment(session, obj));
                    }
                    else
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].CommentId == ((Thread)obj.Comment.Threads[0]).ParentComment.Id)
                            {
                                result.Insert(i + 1, new TransitPostComment(session, obj));
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
                return new TransitImageComment(session, (ImageComment)session.Load(typeof(DBlog.Data.ImageComment), id));
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
                        result.Insert(0, new TransitImageComment(session, obj));
                    }
                    else
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].CommentId == ((Thread)obj.Comment.Threads[0]).ParentComment.Id)
                            {
                                result.Insert(i + 1, new TransitImageComment(session, obj));
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

        [WebMethod(Description = "Increment hourly counter.")]
        public void IncrementHourlyCounter(string ticket, int count)
        {
            using (DBlog.Data.Hibernate.Session.OpenConnection(GetNewConnection()))
            {
                ISession session = DBlog.Data.Hibernate.Session.Current;

                DateTime utcnow = DateTime.UtcNow;
                DateTime hournow = utcnow.Date.AddHours(utcnow.Hour);

                ITransaction t = session.BeginTransaction();
                try
                {
                    HourlyCounter counter = (HourlyCounter) session.CreateCriteria(typeof(HourlyCounter))
                        .Add(Expression.Eq("DateTime", hournow))
                        .UniqueResult();

                    if (counter == null)
                    {
                        counter = new HourlyCounter();
                        counter.DateTime = hournow;
                        counter.RequestCount = 0;
                    }

                    counter.RequestCount += count;
                    session.Save(counter);
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
                    "SELECT sum(hc.RequestCount), min(hc.DateTime) FROM HourlyCounter hc")
                    .UniqueResult();

                tc.Count = (int) result[0];
                tc.Created = (DateTime) result[1];

                return tc;
            }
        }

        #endregion


    }
}