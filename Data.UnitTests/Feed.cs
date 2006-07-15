using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class FeedTest : NHibernateCrudTest
    {
        private Feed mFeed = null;

        public Feed Feed
        {
            get
            {
                return mFeed;
            }
        }

        public FeedTest()
        {
            mFeed = new Feed();
            mFeed.Description = Guid.NewGuid().ToString();
            mFeed.Exception = Guid.NewGuid().ToString();
            mFeed.Interval = 0;
            mFeed.Name = Guid.NewGuid().ToString();
            mFeed.Password = Guid.NewGuid().ToString();
            mFeed.Saved = DateTime.UtcNow;
            mFeed.Type = Guid.NewGuid().ToString();
            mFeed.Updated = DateTime.UtcNow;
            mFeed.Url = Guid.NewGuid().ToString();
            mFeed.Username = Guid.NewGuid().ToString();
            mFeed.Xsl = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mFeed;
            }
        }
    }
}
