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
    public class BlogAssociatedCommentTest : BlogTest
    {
        [Test]
        public void TestGetAssociatedComments()
        {
            int count = Blog.GetAssociatedCommentsCount(Ticket, null);
            Console.WriteLine("Count: {0}", count);           
            Assert.IsTrue(count >= 0);
            TransitAssociatedComment[] comments = Blog.GetAssociatedComments(Ticket, null);
            Assert.IsNotNull(comments);
            Assert.IsTrue(comments.Length >= 0);
            Console.WriteLine("Comments: {0}", comments.Length);
            Assert.AreEqual(comments.Length, count);
        }
    }
}
