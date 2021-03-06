using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using System.Drawing;
using DBlog.Tools.Drawing;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;
using DBlog.Tools.Drawing.Exif;

namespace DBlog.TransitData
{
    public class TransitImageQueryOptions : WebServiceQueryOptions
    {
        private bool mExcludeBlogImages = false;

        public bool ExcludeBlogImages
        {
            get
            {
                return mExcludeBlogImages;
            }
            set
            {
                mExcludeBlogImages = value;
            }
        }

        public TransitImageQueryOptions()
        {
        }

        public TransitImageQueryOptions(
            bool excludeblogimages)
        {
            mExcludeBlogImages = excludeblogimages;
        }

        public TransitImageQueryOptions(
            bool excludeblogimages,
            int pagesize, 
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mExcludeBlogImages = excludeblogimages;
        }

        public override void Apply(ICriteria criteria)
        {
            if (mExcludeBlogImages)
            {
                criteria.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM PostImage e WHERE e.Image_Id = this_.Image_Id )"));
                criteria.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM Highlight h WHERE h.Image_Id = this_.Image_Id )"));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (mExcludeBlogImages)
            {
                query.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM PostImage e WHERE e.Image_Id = this_.Image_Id )"));
                query.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM Highlight h WHERE h.Image_Id = this_.Image_Id )"));
            }

            base.Apply(query);
        }
    }

    public class TransitImage : TransitObject
    {
        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        private string mPath;

        public string Path
        {
            get
            {
                return mPath;
            }
            set
            {
                mPath = value;
            }
        }

        private string mDescription;

        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
            }
        }

        private bool mPreferred;

        public bool Preferred
        {
            get
            {
                return mPreferred;
            }
            set
            {
                mPreferred = value;
            }
        }

        private byte[] mData = null;

        public byte[] Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }

        private byte[] mThumbnail = null;

        public byte[] Thumbnail
        {
            get
            {
                return mThumbnail;
            }
            set
            {
                mThumbnail = value;
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

        public TransitImage()
        {

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

        public TransitImage(ISession session, DBlog.Data.Image o, string ticket)
            : this(session, o, false, false, HasAccess(session, o, ticket))
        {

        }

        public static bool HasAccess(ISession session, DBlog.Data.Image image, string ticket)
        {
            if (image.PostImages == null || image.PostImages.Count == 0)
                return true;

            foreach (PostImage pi in image.PostImages)
            {
                if (TransitPost.GetAccess(session, pi.Post, ticket))
                    return true;
            }

            return false;
        }

        public TransitImage(ISession session, DBlog.Data.Image o, bool withthumbnail, bool withdata, string ticket)
            : this(session, o, withthumbnail, withdata, HasAccess(session, o, ticket))
        {

        }

        public TransitImage(ISession session, DBlog.Data.Image o, bool withthumbnail, bool withdata, bool hasaccess)
            : base(o.Id)
        {
            Name = o.Name;
            Path = o.Path;

            Counter = TransitCounter.GetAssociatedCounter<DBlog.Data.Image, ImageCounter>(
                session, o.Id);

            if (hasaccess)
            {
                Preferred = o.Preferred;
                Description = o.Description;
                Modified = o.Modified;

                CommentsCount = 0;
                
                // hack: disable comments, using disqus
                //CommentsCount = new CountQuery(session, typeof(ImageComment), "ImageComment")
                //    .Add(Expression.Eq("Image.Id", o.Id))
                //    .Execute<int>();

                if (withthumbnail)
                {
                    Thumbnail = o.Thumbnail;
                }

                if (withdata)
                {
                    Data = o.Data;
                }
            }
        }

        public DBlog.Data.Image GetImage(ISession session)
        {
            return GetImage(session, true);
        }

        public DBlog.Data.Image GetImage(ISession session, bool withdata)
        {
            DBlog.Data.Image image = (Id != 0) ? (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), Id) : new DBlog.Data.Image();
            image.Name = Name;
            image.Description = Description;
            image.Modified = DateTime.UtcNow;
            image.Path = Path;
            image.Preferred = Preferred;

            if (withdata)
            {
                image.Data = Data;
                image.Thumbnail = Thumbnail;

                if (image.Thumbnail == null && image.Data != null)
                {
                    image.Thumbnail = new ThumbnailBitmap(image.Data).Thumbnail;
                }
            }

            return image;
        }

    }
}
