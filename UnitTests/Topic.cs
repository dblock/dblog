using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.UnitTests
{
    [TestFixture]
    public class TopicTest : NHibernateCrudTest
    {
        private Topic mTopic = null;

        public Topic Topic
        {
            get
            {
                return mTopic;
            }
        }

        public TopicTest()
        {
            mTopic = new Topic();

            mTopic.Name = Guid.NewGuid().ToString();
            mTopic.Type = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mTopic;
            }
        }
    }
}
