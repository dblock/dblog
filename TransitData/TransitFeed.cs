using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using DBlog.Data.Hibernate;
using NHibernate.Expression;

namespace DBlog.TransitData
{
    public enum TransitFeedType
    {
        Unknown = 0,
        Rss = 1,
        Atom = 2,
        AtomPost = 3,
        ZenFlashGallery = 4,
    }

    public class TransitFeedQueryOptions : WebServiceQueryOptions
    {
        private TransitFeedType mType = TransitFeedType.Unknown;

        public TransitFeedType Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        public TransitFeedQueryOptions()
        {
        }

        public TransitFeedQueryOptions(
            TransitFeedType type)
        {
            mType = type;
        }

        public TransitFeedQueryOptions(
            TransitFeedType type,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mType = type;
        }

        public TransitFeedQueryOptions(
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {

        }

        public override void Apply(ICriteria criteria)
        {
            if (Type != TransitFeedType.Unknown)
            {
                criteria.Add(Expression.Eq("Type", Type.ToString()));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (Type != TransitFeedType.Unknown)
            {
                query.Add(Expression.Eq("Type", Type.ToString()));
            }

            base.Apply(query);
        }
    }

    public class TransitFeed : TransitObject
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

        private int mInterval;

        public int Interval
        {
            get
            {
                return mInterval;
            }
            set
            {
                mInterval = value;
            }
        }

        private DateTime mUpdated;

        public DateTime Updated
        {
            get
            {
                return mUpdated;
            }
            set
            {
                mUpdated = value;
            }
        }

        private string mException;

        public string Exception
        {
            get
            {
                return mException;
            }
            set
            {
                mException = value;
            }
        }

        private string mXsl;

        public string Xsl
        {
            get
            {
                return mXsl;
            }
            set
            {
                mXsl = value;
            }
        }

        private string mUsername;

        public string Username
        {
            get
            {
                return mUsername;
            }
            set
            {
                mUsername = value;
            }
        }

        private string mPassword;

        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                mPassword = value;
            }
        }

        private TransitFeedType mType;

        public TransitFeedType Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        private DateTime mSaved;

        public DateTime Saved
        {
            get
            {
                return mSaved;
            }
            set
            {
                mSaved = value;
            }
        }

        public TransitFeed()
        {

        }

        public TransitFeed(DBlog.Data.Feed o)
            : base(o.Id)
        {
            Url = o.Url;
            Name = o.Name;
            Description = o.Description;
            Interval = o.Interval;
            Updated = o.Updated;
            Exception = o.Exception;
            Xsl = o.Xsl;
            Username = o.Username;
            Password = o.Password;
            Type = (TransitFeedType) Enum.Parse(typeof(TransitFeedType), o.Type);
            Saved = o.Saved;
        }

        public Feed GetFeed(ISession session)
        {
            Feed feed = (Id != 0) ? (Feed)session.Load(typeof(Feed), Id) : new Feed();
            feed.Name = Name;
            feed.Url = Url;
            feed.Description = Description;
            feed.Updated = Updated;
            feed.Interval = Interval;
            feed.Exception = Exception;
            feed.Xsl = Xsl;
            feed.Username = Username;
            feed.Password = Password;
            feed.Type = Type.ToString();
            feed.Saved = Saved;
            return feed;
        }
    }
}
