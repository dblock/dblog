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
    public class BlogReferrerHostRollupTest : BlogCrudTest
    {
        private TransitReferrerHostRollup mReferrerHostRollup = null;

        public BlogReferrerHostRollupTest()
        {
            mReferrerHostRollup = new TransitReferrerHostRollup();
            mReferrerHostRollup.Name = Guid.NewGuid().ToString();
            mReferrerHostRollup.Rollup = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mReferrerHostRollup;
            }
            set
            {
                mReferrerHostRollup = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "ReferrerHostRollup";
            }
        }
    }
}
