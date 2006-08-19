using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;
using DBlog.TransitData.References;
using System.Configuration;
using DBlog.Tools.Web;

namespace DBlog.TransitData
{
    public class TransitPostQueryOptions : WebServiceQueryOptions
    {
        private int mTopicId = 0;
        private string mQuery = string.Empty;

        public int TopicId
        {
            get
            {
                return mTopicId;
            }
            set
            {
                mTopicId = value;
            }
        }

        public string Query
        {
            get
            {
                return mQuery;
            }
            set
            {
                mQuery = value;
            }
        }

        public TransitPostQueryOptions()
        {
        }

        public TransitPostQueryOptions(
            int topicid, string query)
        {
            mTopicId = topicid;
            mQuery = query;
        }

        public TransitPostQueryOptions(
            int topicid,
            string query,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mTopicId = topicid;
            mQuery = query;
        }

        public TransitPostQueryOptions(
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {

        }

        public override void Apply(StringCriteria criteria)
        {
            if (TopicId != 0)
            {
                criteria.Add(string.Format("Post.Topic_Id = {0}", TopicId));
            }

            if (! string.IsNullOrEmpty(Query))
            {
                criteria.Add(string.Format("( FREETEXT (Post.Title, '{0}') OR FREETEXT (Post.Body, '{0}') )",
                    Renderer.SqlEncode(Query)));
            }

            base.Apply(criteria);
        }

        public override void Apply(ICriteria criteria)
        {
            if (TopicId != 0)
            {
                criteria.Add(Expression.Eq("Topic.Id", TopicId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (TopicId != 0)
            {
                query.Add(Expression.Eq("Topic.Id", TopicId));
            }

            base.Apply(query);
        }
    }

    public class TransitPost : TransitObject
    {
        private TransitCounter mCounter;

        public TransitCounter Counter
        {
            get
            {
                return mCounter;
            }
            set
            {
                mCounter = value;
            }
        }

        private int mImageId = 0;

        public int ImageId
        {
            get
            {
                return mImageId;
            }
            set
            {
                mImageId = value;
            }
        }

        private int mImagesCount = 0;

        public int ImagesCount
        {
            get
            {
                return mImagesCount;
            }
            set
            {
                mImagesCount = value;
            }
        }

        private string mTitle;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
            }
        }

        private string mBodyXHTML;

        public string BodyXHTML
        {
            get
            {
                return mBodyXHTML;
            }
            set
            {
                mBodyXHTML = value;
            }
        }

        private string mBody;

        public string Body
        {
            get
            {
                return mBody;
            }
            set
            {
                mBody = value;
            }
        }

        private string mTopicName;

        public string TopicName
        {
            get
            {
                return mTopicName;
            }
            set
            {
                mTopicName = value;
            }
        }

        private int mTopicId;

        public int TopicId
        {
            get
            {
                return mTopicId;
            }
            set
            {
                mTopicId = value;
            }
        }

        private int mLoginId;

        public int LoginId
        {
            get
            {
                return mLoginId;
            }
            set
            {
                mLoginId = value;
            }
        }

        private DateTime mCreated;

        public DateTime Created
        {
            get
            {
                return mCreated;
            }
            set
            {
                mCreated = value;
            }
        }

        private DateTime mModified;

        public DateTime Modified
        {
            get
            {
                return mModified;
            }
            set
            {
                mModified = value;
            }
        }

        private int mCommentsCount = 0;

        public int CommentsCount
        {
            get
            {
                return mCommentsCount;
            }
            set
            {
                mCommentsCount = value;
            }
        }

        public TransitPost()
        {

        }

        public TransitPost(ISession session, DBlog.Data.Post o, string ticket)
            : this(session, o, HasAccess(session, o, ticket))
        {

        }

        public static bool HasAccess(ISession session, Post post, string ticket)
        {
            if (post.PostLogins == null || post.PostLogins.Count == 0)
                return true;

            if (string.IsNullOrEmpty(ticket))
                return false;

            if (ManagedLogin.IsAdministrator(session, ticket))
                return true;

            int login_id = ManagedLogin.GetLoginId(ticket);
            foreach (PostLogin pl in post.PostLogins)
            {
                if (pl.Login.Id == login_id)
                {
                    return true;
                }
            }

            return false;
        }

        public TransitPost(ISession session, DBlog.Data.Post o, bool hasaccess)
            : base(o.Id)
        {
            Title = o.Title;

            if (hasaccess)
            {
                Body = Render(session, o.Id, o.Body);
                BodyXHTML = RenderXHTML(session, o);
            }

            LoginId = o.Login.Id;
            TopicId = o.Topic.Id;
            TopicName = o.Topic.Name;

            if (o.PostImages != null && o.PostImages.Count > 0)
            {
                ImagesCount = o.PostImages.Count;

                if (hasaccess)
                {
                    ImageId = ((PostImage)TransitObject.GetRandomElement(o.PostImages)).Image.Id;
                }
            }

            Created = o.Created;
            Modified = o.Modified;

            CommentsCount = new CountQuery(session, typeof(PostComment), "PostComment")
                .Add(Expression.Eq("Post.Id", o.Id))
                .Execute();

            Counter = TransitCounter.GetAssociatedCounter<Post, PostCounter>(
                session, o.Id);
        }

        public static string Render(ISession session, int id, string value)
        {
            value = new ReferencesRenderer(session, id, "Post").Render(value);
            value = new LiveJournalRenderer(session, id, "Post").Render(value);
            value = new MsnSpacesRenderer(session, id, "Post").Render(value);
            return value;
        }

        public Post GetPost(ISession session)
        {
            Post post = (Id != 0) ? (Post)session.Load(typeof(Post), Id) : new Post();
            post.Title = Title;
            post.Body = Body;
            post.Created = Created;
            post.Login = (LoginId > 0) ? (Login)session.Load(typeof(Login), LoginId) : null;
            post.Topic = (Topic)session.Load(typeof(Topic), TopicId);
            return post;
        }

        public static string RenderXHTML(ISession session, Post post)
        {
            StringBuilder content = new StringBuilder();

            if (!string.IsNullOrEmpty(post.Body))
            {
                content.Append("<div>");
                content.Append(TransitPost.Render(session, post.Id, post.Body));
                content.Append("</div>");
            }

            content.Append("<div>");
            content.AppendFormat("<a href=\"{0}ShowPost.aspx?id={1}\">Read</a>",
                ConfigurationManager.AppSettings["url"], post.Id);

            if (post.PostImages != null && post.PostImages.Count > 1)
            {
                content.AppendFormat(" | <a href=\"{0}ShowPost.aspx?id={1}\">{2} Images</a>",
                    ConfigurationManager.AppSettings["url"], post.Id, post.PostImages.Count);
            }

            if (post.Created != post.Modified)
            {
                content.AppendFormat("| Updated {0}",
                    post.Modified.ToString("d"));
            }

            if (post.PostImages != null && post.PostImages.Count > 0)
            {
                if (post.PostLogins == null || post.PostLogins.Count == 0)
                {
                    content.Append("<div style=\"margin-top: 10px;\">");
                    for (int i = 0; i < Math.Min(3, post.PostImages.Count); i++)
                    {
                        content.AppendFormat("<img src=\"{0}ShowPicture.aspx?id={1}\">",
                            ConfigurationManager.AppSettings["url"], ((PostImage)post.PostImages[i]).Image.Id);
                    }
                    content.Append("</div>");
                }
            }

            content.Append("</div>");
            return content.ToString();
        }
    }
}
