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
    public class BlogPermalinkTest : BlogCrudTest
    {
        private TransitPermalink mPermalink = null;

        public BlogPermalinkTest()
        {
            mPermalink = new TransitPermalink();
            mPermalink.SourceId = new Random().Next();
            mPermalink.TargetId = new Random().Next();
            mPermalink.SourceType = Guid.NewGuid().ToString();
            mPermalink.TargetType = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mPermalink;
            }
            set
            {
                mPermalink = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Permalink";
            }
        }
    }
}
