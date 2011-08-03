using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using DBlog.Data.Hibernate;
using DBlog.TransitData.References;
using System.Configuration;
using DBlog.Tools.Web;
using DBlog.Tools.Web.Html;

namespace DBlog.TransitData
{
    public class TransitPostQueryOptions : WebServiceQueryOptions
    {
        private int mTopicId = 0;
        private bool mPublishedOnly = true;
        private bool mDisplayedOnly = true;
        private string mQuery = string.Empty;
        private DateTime mDateStart = DateTime.MinValue;
        private DateTime mDateEnd = DateTime.MaxValue;

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

        public DateTime DateStart
        {
            get
            {
                return mDateStart;
            }
            set
            {
                mDateStart = value;
            }
        }

        public DateTime DateEnd
        {
            get
            {
                return mDateEnd;
            }
            set
            {
                mDateEnd = value;
            }
        }

        public bool PublishedOnly
        {
            get
            {
                return mPublishedOnly;
            }
            set
            {
                mPublishedOnly = value;
            }
        }

        public bool DisplayedOnly
        {
            get
            {
                return mDisplayedOnly;
            }
            set
            {
                mDisplayedOnly = value;
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
            if (!string.IsNullOrEmpty(Query))
            {
                criteria.AddJoin(string.Format("INNER JOIN FREETEXTTABLE(Post, (Title, Body), '{0}') AS KEY_TBL ON Post.Post_Id = KEY_TBL.[KEY]",
                    Renderer.SqlEncode(Query)));
            }

            if (TopicId != 0)
            {
                criteria.Add(string.Format("EXISTS (SELECT * FROM PostTopic WHERE PostTopic.Post_Id = Post.Post_Id AND PostTopic.Topic_Id = {0})",
                    TopicId));
            }

            if (DateStart != DateTime.MinValue)
            {
                criteria.Add(string.Format("Created >= '{0}'", DateStart));
            }

            if (DateEnd != DateTime.MaxValue)
            {
                criteria.Add(string.Format("Created <= '{0}'", DateEnd));
            }

            if (PublishedOnly)
            {
                criteria.Add("Publish = 1");
            }

            if (DisplayedOnly)
            {
                criteria.Add("Display = 1");
            }

            if (string.IsNullOrEmpty(Query))
            {
                criteria.AddOrder("Sticky", WebServiceQuerySortDirection.Descending);
            }

            base.Apply(criteria);
        }

        public override void Apply(ICriteria criteria)
        {
            if (!string.IsNullOrEmpty(Query))
            {
                criteria.CreateAlias(string.Format("FREETEXTTABLE(Post, (Title, Body), '{0}')", Renderer.SqlEncode(Query)),
                    "KEY_TBL", JoinType.InnerJoin).Add(Restrictions.Eq("Post.Post_Id", "KEY_TBL.[KEY]"));
            }

            if (TopicId != 0)
            {
                criteria.Add(Expression.Sql(string.Format("EXISTS ( SELECT * FROM PostTopic t WHERE t.Post_Id = this_.Post_Id AND t.Topic_Id = {0})",
                    TopicId)));
            }

            if (DateStart != DateTime.MinValue)
            {
                criteria.Add(Expression.Ge("Created", DateStart));
            }

            if (DateEnd != DateTime.MaxValue)
            {
                criteria.Add(Expression.Le("Created", DateEnd));
            }

            if (PublishedOnly)
            {
                criteria.Add(Expression.Eq("Publish", true));
            }

            if (DisplayedOnly)
            {
                criteria.Add(Expression.Eq("Display", true));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (TopicId != 0)
            {
                query.Add(Expression.Sql(string.Format("EXISTS ( SELECT * FROM PostTopic t WHERE t.Post_Id = this_.Post_Id AND t.Topic_Id = {0})",
                    TopicId)));
            }

            if (! string.IsNullOrEmpty(Query))
            {
                query.Add(Expression.Sql(string.Format("( FREETEXT (Title, '{0}') OR FREETEXT (Body, '{0}') )",
                    Renderer.SqlEncode(Query))));
            }

            if (DateStart != DateTime.MinValue)
            {
                query.Add(Expression.Ge("Created", DateStart));
            }

            if (DateEnd != DateTime.MaxValue)
            {
                query.Add(Expression.Le("Created", DateEnd));
            }

            if (PublishedOnly)
            {
                query.Add(Expression.Eq("Publish", true));
            }

            if (DisplayedOnly)
            {
                query.Add(Expression.Eq("Display", true));
            }

            base.Apply(query);
        }
    }

    public class TransitPost : TransitObject
    {
        private TransitTopic[] mTopics;

        public TransitTopic[] Topics
        {
            get
            {
                return mTopics;
            }
            set
            {
                mTopics = value;
            }
        }

        private bool mHasAccess = true;

        public bool HasAccess
        {
            get
            {
                return mHasAccess;
            }
            set
            {
                mHasAccess = value;
            }
        }

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

        private string mSlug;

        public string Slug
        {
            get
            {
                return mSlug;
            }
            set
            {
                mSlug = value;
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

        private string mRawBody;

        public string RawBody
        {
            get
            {
                return mRawBody;
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

        private bool mPublish = true;

        public bool Publish
        {
            get
            {
                return mPublish;
            }
            set
            {
                mPublish = value;
            }
        }

        private bool mDisplay = true;

        public bool Display
        {
            get
            {
                return mDisplay;
            }
            set
            {
                mDisplay = value;
            }
        }

        private bool mSticky = true;

        public bool Sticky
        {
            get
            {
                return mSticky;
            }
            set
            {
                mSticky = value;
            }
        }

        private bool mExport = false;

        public bool Export
        {
            get
            {
                return mExport;
            }
            set
            {
                mExport = value;
            }
        }

        public TransitPost()
        {

        }

        public TransitPost(ISession session, DBlog.Data.Post o, string ticket)
            : this(session, o, GetAccess(session, o, ticket))
        {

        }

        public static bool GetAccess(ISession session, Post post, string ticket)
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
            Slug = o.Slug;
            HasAccess = hasaccess;

            if (hasaccess)
            {
                mRawBody = o.Body;
                Body = Render(session, o.Body);
                BodyXHTML = RenderXHTML(session, o);
            }

            LoginId = o.Login.Id;

            if (o.PostImages != null && o.PostImages.Count > 0)
            {
                ImagesCount = o.PostImages.Count;

                if (hasaccess)
                {
                    ImageId = ((PostImage)TransitObject.GetRandomElement(o.PostImages)).Image.Id;
                }
            }

            // topics
            List<TransitTopic> topics = new List<TransitTopic>();
            if (o.PostTopics != null)
            {
                foreach (PostTopic postTopic in o.PostTopics)
                {
                    topics.Add(new TransitTopic(postTopic.Topic));
                }
            }
            mTopics = topics.ToArray();

            Created = o.Created;
            Modified = o.Modified;

            CommentsCount = new CountQuery(session, typeof(PostComment), "PostComment")
                .Add(Expression.Eq("Post.Id", o.Id))
                .Execute<int>();

            Counter = TransitCounter.GetAssociatedCounter<Post, PostCounter>(
                session, o.Id);

            Publish = o.Publish;
            Display = o.Display;
            Sticky = o.Sticky;
            Export = o.Export;
        }

        public static string Render(ISession session, string value)
        {
            value = new ReferencesRenderer(session).Render(value);
            value = new LiveJournalRenderer(session).Render(value);
            value = new MsnSpacesRenderer(session).Render(value);
            value = Renderer.RenderMarkups(value);
            return value;
        }

        public Post GetPost(ISession session)
        {
            Post post = (Id != 0) ? (Post)session.Load(typeof(Post), Id) : new Post();
            post.Title = Title;
            post.Slug = Slug;
            post.Body = Body;
            post.Created = Created;
            post.Login = (LoginId > 0) ? (Login)session.Load(typeof(Login), LoginId) : null;
            post.Publish = Publish;
            post.Display = Display;
            post.Sticky = Sticky;
            post.Export = Export;
            return post;
        }

        public void GenerateSlug(ISession session)
        {
            if (! string.IsNullOrEmpty(Slug))
                return;

            String slug_base = Renderer.ToSlug(Title);
            String slug_candidate = "";
            int slug_count = 0;
            Post existing_post = null;

            do
            {
                slug_candidate = slug_base + (slug_count == 0 ? "" : string.Format("-{0}", slug_count));
                existing_post = session.CreateCriteria(typeof(Post))
                    .Add(Expression.Eq("Slug", slug_candidate))
                    .Add(Expression.Not(Expression.Eq("Id", this.Id)))
                    .UniqueResult<Post>();
                slug_count += 1;
            } while (existing_post != null);

            Slug = slug_candidate;
        }

        public static string RenderXHTML(ISession session, Post post)
        {
            StringBuilder content = new StringBuilder();

            if (!string.IsNullOrEmpty(post.Body))
            {
                content.Append("<div>");
                string body = Cutter.DeleteCut(post.Body);
                body = Render(session, body);
                body = Renderer.RenderEx(body, ConfigurationManager.AppSettings["url"], null);
                body = body.Replace("&", "&amp;");
                content.Append(body);
                content.Append("</div>");
            }

            content.Append("<div>");

            string link = string.IsNullOrEmpty(post.Slug)
                ? string.Format("ShowPost.aspx?id={0}", post.Id)
                : string.Format("{0}", post.Slug);
            content.AppendFormat("<a href=\"{0}\">Read</a>",
                link);

            if (post.PostImages != null && post.PostImages.Count > 1)
            {
                content.AppendFormat(" | <a href=\"ShowPost.aspx?id={0}\">{1} Image{2}</a>",
                    post.Id, post.PostImages.Count, post.PostImages.Count != 1 ? "s" : string.Empty);
            }

            if (post.Created != post.Modified)
            {
                content.AppendFormat(" | Updated {0}",
                    post.Modified.ToString("d"));
            }

            if (post.PostImages != null && post.PostImages.Count > 0)
            {
                if (post.PostLogins == null || post.PostLogins.Count == 0)
                {
                    content.Append("<div style=\"margin-top: 10px;\">");
                    for (int i = 0; i < Math.Min(3, post.PostImages.Count); i++)
                    {
                        content.AppendFormat("<img src=\"ShowPicture.aspx?id={0}\">",
                            ((PostImage)post.PostImages[i]).Image.Id);
                    }
                    content.Append("</div>");
                }
            }

            content.Append("</div>");

            Uri root;
            if (Uri.TryCreate(ConfigurationManager.AppSettings["url"], UriKind.Absolute, out root))
            {
                return Tools.Web.Html.HtmlAbsoluteLinksWriter.Rewrite(
                    content.ToString(), root);
            }
            else
            {
                return content.ToString();
            }
        }

        public string LinkUri
        {
            get
            {
                return string.IsNullOrEmpty(Slug)
                    ? string.Format("ShowPost.aspx?id={0}", Id)
                    : string.Format("{0}", Slug);
            }
        }
    }
}
