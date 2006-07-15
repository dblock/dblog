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
    public class FeedItemTest : NHibernateCrudTest
    {
        private FeedItem mFeedItem = null;

        public FeedItem FeedItem
        {
            get
            {
                return mFeedItem;
            }
        }

        public FeedItemTest()
        {
            FeedTest feed = new FeedTest();
            AddDependentObject(feed);

            mFeedItem = new FeedItem();
            mFeedItem.Description = Guid.NewGuid().ToString();
            mFeedItem.Feed = feed.Feed;
            mFeedItem.Link = Guid.NewGuid().ToString();
            mFeedItem.Title = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mFeedItem;
            }
        }
    }
}
