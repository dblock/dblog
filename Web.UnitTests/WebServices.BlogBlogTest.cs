using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using DBlog.Web.UnitTests.WebServices.Blog;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogBlogTest : BlogTest
    {
        private string mTicket = string.Empty;

        public string Ticket
        {
            get
            {
                if (string.IsNullOrEmpty(mTicket))
                {
                    mTicket = Blog.Login("Administrator", string.Empty);
                }

                return mTicket;
            }
        }

        [Test]
        public void GetBlogsByTopicTest()
        {
            BlogTopicTest topic = new BlogTopicTest();
            topic.SetUp();
            int topic_id = topic.Create();

            TransitBlogQueryOptions qo = new TransitBlogQueryOptions();
            qo.TopicId = topic_id;
            qo.PageNumber = 0;
            qo.PageSize = 10;

            int count = Blog.GetBlogsCount(Ticket, qo);
            TransitBlog[] blogs = Blog.GetBlogs(Ticket, qo);

            Assert.AreEqual(count, blogs.Length);
            Console.WriteLine("Blogs: {0}", blogs.Length);

            topic.Delete();
            topic.TearDown();
        }
    }
}
