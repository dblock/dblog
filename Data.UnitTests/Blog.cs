using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class BlogTest : NHibernateTest
    {
        public BlogTest()
        {
        }

        [Test]
        public void EntryToBlogTest()
        {
            EntryTest entry = new EntryTest();
            entry.Session = Session;
            int id = entry.Create();
            Blog blog = (Blog) Session.Load(typeof(Blog), id);
            Console.WriteLine(string.Format("Blog entry: {0}", blog.Id));
            Assert.IsFalse(id == 0, "No blog view object saved.");
            Assert.AreEqual(entry.Entry.Id, blog.Id, "Invalid blog view object retreived.");
            entry.Delete();
        }
    }
}
