using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostImageQueryOptions : WebServiceQueryOptions
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

        public TransitPostImageQueryOptions()
        {
        }

        public TransitPostImageQueryOptions(
            int entryid)
        {
            mPostId = entryid;
        }

        public TransitPostImageQueryOptions(
            int entryid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mPostId = entryid;
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


    public class TransitPostImage : TransitObject
    {
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

        public TransitPostImage(ISession session, DBlog.Data.PostImage o)
            : base(o.Id)
        {
            Post = new TransitPost(session, o.Post);
            Image = new TransitImage(session, o.Image);
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
