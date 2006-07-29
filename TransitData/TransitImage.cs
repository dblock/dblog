using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using System.Drawing;
using DBlog.Tools.Drawing;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

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
                criteria.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM PostImage e WHERE e.Image_Id = this.Image_Id )"));
                criteria.Add(Expression.Sql("NOT EXISTS ( SELECT * FROM Highlight h WHERE h.Image_Id = this.Image_Id )"));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (mExcludeBlogImages)
            {
                query.Add(Expression.Sql("NOT EXISTS ( SELECT e.Image.Id FROM PostImage e WHERE e.Image.Id = Image.Id )"));
                query.Add(Expression.Sql("NOT EXISTS ( SELECT h.Image.Id FROM Highlight h WHERE h.Image.Id = Image.Id )"));
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

        public TransitImage(ISession session, DBlog.Data.Image o)
            : this(session, o, false, false)
        {

        }

        public TransitImage(ISession session, DBlog.Data.Image o, bool withthumbnail, bool withdata)
            : base(o.Id)
        {
            Path = o.Path;
            Name = o.Name;
            Preferred = o.Preferred;
            Description = o.Description;
            Modified = o.Modified;

            CommentsCount = new CountQuery(session, typeof(ImageComment), "ImageComment")
                .Add(Expression.Eq("Image.Id", o.Id))
                .Execute();

            if (withthumbnail)
            {
                Thumbnail = o.Thumbnail;
            }

            if (withdata)
            {
                Data = o.Data;
            }
        }

        public DBlog.Data.Image GetImage(ISession session)
        {
            DBlog.Data.Image image = (Id != 0) ? (DBlog.Data.Image)session.Load(typeof(DBlog.Data.Image), Id) : new DBlog.Data.Image();
            image.Name = Name;
            image.Description = Description;
            image.Modified = DateTime.UtcNow;
            image.Path = Path;
            image.Preferred = Preferred;
            image.Data = Data;
            image.Thumbnail = Thumbnail;
            
            if (image.Thumbnail == null && image.Data != null)
            {
                image.Thumbnail = new ThumbnailBitmap(image.Data).Thumbnail;
            }

            return image;
        }

    }
}
