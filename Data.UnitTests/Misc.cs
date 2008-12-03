using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data.Hibernate;
using DBlog.Data.Hibernate.UnitTests;
using NHibernate.Expression;
using NUnit.Framework;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class Misc : NHibernateTest
    {
        [Test]
        public void TestCountTopics()
        {
            TopicTest tt = new TopicTest();
            tt.Session = Session;
            tt.Create();

            int count = new CountQuery(Session, typeof(Topic), "Topic")
                .Add(Expression.Eq("Name", tt.Topic.Name))
                .Execute<int>();

            Assert.AreEqual(1, count);

            tt.Delete();
        }
    }
}
