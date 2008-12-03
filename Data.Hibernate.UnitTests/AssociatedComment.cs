using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHibernate;

namespace DBlog.Data.Hibernate.UnitTests
{
    [TestFixture]
    public class AssociatedCommentTests : NHibernateTest
    {
        [Test]
        public void TestGetAssociatedComments()
        {
            IQuery q = Session.GetNamedQuery("GetAssociatedComments");
            Assert.IsNotNull(q);
            IList<AssociatedComment> comments = q.List<AssociatedComment>();
            Assert.IsNotNull(comments);
            Assert.IsTrue(comments.Count >= 0);
            foreach (AssociatedComment comment in comments)
            {
                Console.WriteLine("{0}: {1}", comment.Id, comment.Text);
            }
        }
    }
}
