using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using System.Drawing;
using DBlog.Tools.Drawing;

namespace DBlog.TransitData
{
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

        public TransitImage(DBlog.Data.Image o)
            : this(o, false, false)
        {

        }

        public TransitImage(DBlog.Data.Image o, bool withthumbnail, bool withdata)
            : base(o.Id)
        {
            Path = o.Path;
            Name = o.Name;
            Preferred = o.Preferred;
            Description = o.Description;
            Modified = o.Modified;

            if (withthumbnail)
            {
                Thumbnail = o.Thumbnail;
            }

            if (withdata)
            {
                Data = o.Data;

                if (o.Data == null && !string.IsNullOrEmpty(Path))
                {
                    ThumbnailBitmap bitmap = new ThumbnailBitmap(Path);
                    Data = bitmap.Bitmap;
                }
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
            return image;
        }

    }
}
