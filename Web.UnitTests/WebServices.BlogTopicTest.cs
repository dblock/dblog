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
    public class BlogTopicTest : BlogCrudTest
    {
        private TransitTopic mTopic = null;

        public BlogTopicTest()
        {
            mTopic = new TransitTopic();
            mTopic.Name = Guid.NewGuid().ToString();
            mTopic.Type = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mTopic;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "Topic";
            }
        }
    }
}
