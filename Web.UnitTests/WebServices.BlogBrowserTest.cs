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
    public class BlogBrowserTest : BlogCrudTest
    {
        private TransitBrowser mBrowser = null;

        public BlogBrowserTest()
        {
            mBrowser = new TransitBrowser();
            mBrowser.Name = Guid.NewGuid().ToString();
            mBrowser.Platform = Guid.NewGuid().ToString();
            mBrowser.Version = "1.0";
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mBrowser;
            }
            set
            {
                mBrowser = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Browser";
            }
        }
    }
}
