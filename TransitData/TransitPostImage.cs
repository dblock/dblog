using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostImageQueryOptions : WebServiceQueryOptions
    {
        private int mPostId = 0;
        private bool mPreferredOnly = false;
        private bool mCounters = false;

        public bool Counters
        {
            get
            {
                return mCounters;
            }
            set
            {
                mCounters = value;
            }
        }

        public bool PreferredOnly
        {
            get
            {
                return mPreferredOnly;
            }
            set
            {
                mPreferredOnly = value;
            }
        }

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

        public TransitPostImageQueryOptions()
        {
        }

        public TransitPostImageQueryOptions(
            int postid)
        {
            mPostId = postid;
        }

        public TransitPostImageQueryOptions(
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

            if (PreferredOnly)
            {
                throw new NotImplementedException();
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (PostId != 0)
            {
                query.Add(Expression.Eq("Post.Id", PostId));
            }

            if (PreferredOnly)
            {
                throw new NotImplementedException();
            }

            base.Apply(query);
        }
    }


    public class TransitPostImage : TransitObject
    {
        private int mIndex = 0;

        public int Index
        {
            get
            {
                return mIndex;
            }
            set
            {
                mIndex = value;
            }
        }

        private TransitImage mImage = null;

        public TransitImage Image
        {
            get
            {
                return mImage;
            }
            set
            {
                mImage = value;
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

        public TransitPostImage()
        {

        }

        public TransitPostImage(ISession session, DBlog.Data.PostImage o, string ticket)
            : base(o.Id)
        {
            Post = new TransitPost(session, o.Post, ticket);
            Image = new TransitImage(session, o.Image, ticket);
        }

        public PostImage GetPostImage(ISession session)
        {
            PostImage ei = (Id != 0) ? (PostImage)session.Load(typeof(PostImage), Id) : new PostImage();
            ei.Image = (Image)session.Load(typeof(Image), Image.Id);
            ei.Post = (Post)session.Load(typeof(Post), Post.Id);
            return ei;
        }
    }
}
