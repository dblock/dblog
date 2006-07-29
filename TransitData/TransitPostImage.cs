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
        private int mImageId;

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

        private int mPostId;

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

        private string mImageName;

        public string ImageName
        {
            get
            {
                return mImageName;
            }
            set
            {
                mImageName = value;
            }
        }

        private string mImageDescription;

        public string ImageDescription
        {
            get
            {
                return mImageDescription;
            }
            set
            {
                mImageDescription = value;
            }
        }

        private int mImageCommentsCount = 0;

        public int ImageCommentsCount
        {
            get
            {
                return mImageCommentsCount;
            }
            set
            {
                mImageCommentsCount = value;
            }
        }

        public TransitPostImage()
        {

        }

        public TransitPostImage(ISession session, DBlog.Data.PostImage o)
            : base(o.Id)
        {
            PostId = o.Post.Id;
            ImageId = o.Image.Id;
            ImageDescription = o.Image.Description;
            ImageName = o.Image.Name;
            ImageCommentsCount = new CountQuery(session, typeof(ImageComment), "ImageComment")
                .Add(Expression.Eq("Image.Id", o.Image.Id))
                .Execute();
        }

        public PostImage GetPostImage(ISession session)
        {
            PostImage ei = (Id != 0) ? (PostImage)session.Load(typeof(PostImage), Id) : new PostImage();
            ei.Image = (Image)session.Load(typeof(Image), ImageId);
            ei.Post = (Post)session.Load(typeof(Post), PostId);
            return ei;
        }
    }
}
