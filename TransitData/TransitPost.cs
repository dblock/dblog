using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostQueryOptions : WebServiceQueryOptions
    {
        private int mTopicId = 0;

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

        public TransitPostQueryOptions()
        {
        }

        public TransitPostQueryOptions(
            int topicid)
        {
            mTopicId = topicid;
        }

        public TransitPostQueryOptions(
            int topicid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mTopicId = topicid;
        }

        public TransitPostQueryOptions(
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {

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

        public TransitPost(ISession session, DBlog.Data.Post o)
            : base(o.Id)
        {
            Title = o.Title;
            Body = o.Body;
            LoginId = o.Login.Id;
            TopicId = o.Topic.Id;
            
            if (o.PostImages != null && o.PostImages.Count > 0)
            {
                ImagesCount = o.PostImages.Count;
                ImageId = ((PostImage)TransitObject.GetRandomElement(o.PostImages)).Image.Id;
            }

            Created = o.Created;
            Modified = o.Modified;

            CommentsCount = new CountQuery(session, typeof(PostComment), "PostComment")
                .Add(Expression.Eq("Post.Id", o.Id))
                .Execute();

            Counter = TransitCounter.GetAssociatedCounter<Post, PostCounter>(
                session, o.Id);
        }

        public Post GetPost(ISession session)
        {
            Post post = (Id != 0) ? (Post)session.Load(typeof(Post), Id) : new Post();
            post.Title = Title;
            post.Body = Body;
            post.Login = (LoginId > 0) ? (Login)session.Load(typeof(Login), LoginId) : null;
            post.Topic = (Topic)session.Load(typeof(Topic), TopicId);
            return post;
        }
    }
}
