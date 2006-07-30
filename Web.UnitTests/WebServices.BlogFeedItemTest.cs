using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Reflection;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogFeedItemTest : BlogCrudTest
    {
        private TransitFeedItem mFeedItem = null;
        private BlogFeedTest mFeedTest = null;

        public override bool IsServiceCount
        {
            get
            {
                return true;
            }
        }

        public BlogFeedItemTest()
        {
            mFeedItem = new TransitFeedItem();
            mFeedItem.Title = Guid.NewGuid().ToString();
            mFeedItem.Description = Guid.NewGuid().ToString();
            mFeedItem.Link = Guid.NewGuid().ToString();

            mFeedTest = new BlogFeedTest();
            AddDependent(mFeedTest);
        }

        public override int Create()
        {
            mFeedItem.FeedId = mFeedTest.Create();
            return base.Create();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mFeedItem;
            }
            set
            {
                mFeedItem = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "FeedItem";
            }
        }

        public override void Delete()
        {
            base.Delete();
            mFeedTest.Delete();
        }
    }
}
