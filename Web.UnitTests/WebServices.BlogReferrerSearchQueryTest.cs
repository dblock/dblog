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
    public class BlogReferrerSearchQueryTest : BlogCrudTest
    {
        private TransitReferrerSearchQuery mReferrerSearchQuery = null;

        public BlogReferrerSearchQueryTest()
        {
            mReferrerSearchQuery = new TransitReferrerSearchQuery();
            mReferrerSearchQuery.SearchQuery = Guid.NewGuid().ToString();
            mReferrerSearchQuery.RequestCount = 1;
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mReferrerSearchQuery;
            }
            set
            {
                mReferrerSearchQuery = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "ReferrerSearchQuery";
            }
        }

        public override string ObjectsType
        {
            get
            {
                return "ReferrerSearchQueries";
            }
        }
    }
}
