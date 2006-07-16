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
            CountQuery q = new CountQuery(Session, typeof(Blog), "Blog");
            q.Add(Expression.Eq("Topic.Id", "10"));
            q.Add(Expression.Eq("Title", "Foo"));
            q.Add(Expression.Eq("Title", "Ba'r"));
            q.Add(Expression.IsNotNull("Title"));
            string qs = q.ToString();
            Console.WriteLine(qs);
            Assert.AreEqual(
                "SELECT COUNT(Blog) FROM Blog Blog" + 
                " WHERE Blog.Topic.Id = '10'" +
                " AND Blog.Title = 'Foo'" +
                " AND Blog.Title = 'Ba''r'" +
                " AND Blog.Title is not null",
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
                .execute();

            Assert.AreEqual(1, count);

            tt.Delete();
        }
    }
}
