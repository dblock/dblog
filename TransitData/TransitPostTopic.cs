using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostTopicQueryOptions : WebServiceQueryOptions
    {
        private int mPostId = 0;

        public int PostId
        {
            get
            {
                return mPostId;
            }
            set
            {
                mPostId = value;
            }
        }

        public TransitPostTopicQueryOptions()
        {
        }

        public TransitPostTopicQueryOptions(
            int postid)
        {
            mPostId = postid;
        }

        public TransitPostTopicQueryOptions(
            int postid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mPostId = postid;
        }

        public override void Apply(ICriteria criteria)
        {
            if (PostId != 0)
            {
                criteria.Add(Expression.Eq("Post.Id", PostId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (PostId != 0)
            {
                query.Add(Expression.Eq("Post.Id", PostId));
            }

            base.Apply(query);
        }
    }


    public class TransitPostTopic : TransitObject
    {
        private TransitTopic mTopic = null;

        public TransitTopic Topic
        {
            get
            {
                return mTopic;
            }
            set
            {
                mTopic = value;
            }
        }

        private TransitPost mPost = null;

        public TransitPost Post
        {
            get
            {
                return mPost;
            }
            set
            {
                mPost = value;
            }
        }

        public TransitPostTopic()
        {

        }

        public TransitPostTopic(ISession session, DBlog.Data.PostTopic o)
            : base(o.Id)
        {
            Post = new TransitPost(session, o.Post, false);
            Topic = new TransitTopic(o.Topic);
        }

        public PostTopic GetPostTopic(ISession session)
        {
            PostTopic ei = (Id != 0) ? (PostTopic)session.Load(typeof(PostTopic), Id) : new PostTopic();
            ei.Topic = (Topic)session.Load(typeof(Topic), Topic.Id);
            ei.Post = (Post)session.Load(typeof(Post), Post.Id);
            return ei;
        }
    }
}
