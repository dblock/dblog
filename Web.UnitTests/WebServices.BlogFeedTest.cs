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
    public class BlogFeedTest : BlogCrudTest
    {
        private TransitFeed mFeed = null;

        public BlogFeedTest()
        {
            mFeed = new TransitFeed();
            mFeed.Name = Guid.NewGuid().ToString();
            mFeed.Description = Guid.NewGuid().ToString();
            mFeed.Username = Guid.NewGuid().ToString();
            mFeed.Password = Guid.NewGuid().ToString();
            mFeed.Saved = DateTime.UtcNow;
            mFeed.Updated = DateTime.UtcNow;
            mFeed.Xsl = Guid.NewGuid().ToString();
            mFeed.Exception = Guid.NewGuid().ToString();
            mFeed.Url = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mFeed;
            }
            set
            {
                mFeed = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Feed";
            }
        }
    }
}
