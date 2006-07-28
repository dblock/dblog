using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data.Hibernate;
using NHibernate.Expression;
using NUnit.Framework;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class Misc : NHibernateTest
    {
        [Test]
        public void TestCountQuery()
        {
            CountQuery q = new CountQuery(Session, typeof(Post), "Post");
            q.Add(Expression.Eq("Topic.Id", "10"));
            q.Add(Expression.Eq("Title", "Foo"));
            q.Add(Expression.Eq("Title", "Ba'r"));
            q.Add(Expression.IsNotNull("Title"));
            string qs = q.ToString();
            Console.WriteLine(qs);
            Assert.AreEqual(
                "SELECT COUNT(Post) FROM Post Post" + 
                " WHERE Post.Topic.Id = '10'" +
                " AND Post.Title = 'Foo'" +
                " AND Post.Title = 'Ba''r'" +
                " AND Post.Title is not null",
                qs);
        }

        [Test]
        public void TestCountTopics()
        {
            TopicTest tt = new TopicTest();
            tt.Session = Session;
            tt.Create();

            int count = new CountQuery(Session, typeof(Topic), "Topic")
                .Add(Expression.Eq("Name", tt.Topic.Name))
                .Execute();

            Assert.AreEqual(1, count);

            tt.Delete();
        }
    }
}
