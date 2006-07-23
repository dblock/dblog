using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitHighlight : TransitObject
    {
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

        private string mUrl;

        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                mUrl = value;
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

        public TransitHighlight()
        {

        }

        public TransitHighlight(DBlog.Data.Highlight o)
            : base(o.Id)
        {
            Url = o.Url;
            Title = o.Title;
            Description = o.Description;
            ImageId = o.Image.Id;
        }

        public Highlight GetHighlight(ISession session)
        {
            Highlight highlight = (Id != 0) ? (Highlight)session.Load(typeof(Highlight), Id) : new Highlight();
            highlight.Title = Title;
            highlight.Url = Url;
            highlight.Description = Description;
            highlight.Image = (ImageId != 0) ? (Image) session.Load(typeof(Image), ImageId) : null;
            return highlight;
        }
    }
}
