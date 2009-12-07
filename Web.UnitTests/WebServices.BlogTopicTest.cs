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
            set
            {
                mTopic = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "Topic";
            }
        }

        [Test]
        public void TestGetTopicByName()
        {
            TransitTopic t_topic = new TransitTopic();
            t_topic.Name = Guid.NewGuid().ToString();
            t_topic.Type = Guid.NewGuid().ToString();
            Assert.IsNull(Blog.GetTopicByName(Ticket, t_topic.Name));
            t_topic.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic);
            TransitTopic t_topic_2 = Blog.GetTopicByName(Ticket, t_topic.Name);
            Assert.IsNotNull(t_topic_2);
            Assert.AreEqual(t_topic.Name, t_topic_2.Name);
            Blog.DeleteTopic(Ticket, t_topic.Id);
        }
    }
}
