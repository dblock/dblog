using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitReferrerHost : TransitObject
    {
        private string mLastUrl;

        public string LastUrl
        {
            get
            {
                return mLastUrl;
            }
            set
            {
                mLastUrl = value;
            }
        }

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

        private string mLastSource;

        public string LastSource
        {
            get
            {
                return mLastSource;
            }
            set
            {
                mLastSource = value;
            }
        }

        private long mRequestCount;

        public long RequestCount
        {
            get
            {
                return mRequestCount;
            }
            set
            {
                mRequestCount = value;
            }
        }

        public TransitReferrerHost()
        {

        }

        public TransitReferrerHost(DBlog.Data.ReferrerHost o)
            : base(o.Id)
        {
            LastUrl = o.LastUrl;
            LastSource = o.LastSource;
            Name = o.Name;
            RequestCount = o.RequestCount;
        }

        public ReferrerHost GetReferrerHost(ISession session)
        {
            ReferrerHost referrerhost = (Id != 0) ? (ReferrerHost)session.Load(typeof(ReferrerHost), Id) : new ReferrerHost();
            referrerhost.LastUrl = LastUrl;
            referrerhost.Name = Name;
            referrerhost.LastSource = LastSource;
            referrerhost.RequestCount = RequestCount;
            return referrerhost;
        }
    }
}
