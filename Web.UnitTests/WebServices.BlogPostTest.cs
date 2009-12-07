using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using DBlog.Tools.Drawing;
using System.Threading;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogPostTest : BlogCrudTest
    {
        private TransitPost mPost = null;
        private BlogTopicTest mTopic1Test = null;
        private BlogTopicTest mTopic2Test = null;

        public BlogPostTest()
        {
            mPost = new TransitPost();
            mPost.Title = Guid.NewGuid().ToString();
            mPost.Body = Guid.NewGuid().ToString();
            mPost.Created = mPost.Modified = DateTime.UtcNow;

            mTopic1Test = new BlogTopicTest();
            AddDependent(mTopic1Test);
            mTopic2Test = new BlogTopicTest();
            AddDependent(mTopic2Test);
        }

        public override int Create()
        {
            mPost.LoginId = Blog.GetLogin(Ticket).Id;
            mTopic1Test.Create();
            mTopic2Test.Create();
            List<TransitTopic> topics = new List<TransitTopic>();
            topics.Add((TransitTopic) mTopic1Test.TransitInstance);
            topics.Add((TransitTopic) mTopic2Test.TransitInstance);
            return base.Create();
        }

        public override void Delete()
        {
            base.Delete();
            mTopic1Test.Delete();
            mTopic2Test.Delete();
        }

        public override string ObjectsType
        {
            get
            {
                return "Posts";
            }
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mPost;
            }
            set
            {
                mPost = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Post";
            }
        }

        [Test]
        public void CreatePostWithImageTest()
        {
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            TransitImage t_image = new TransitImage();
            t_image.Name = Guid.NewGuid().ToString();
            
            Bitmap b = new Bitmap(480, 480);
            Graphics g = Graphics.FromImage(b);
            g.FillEllipse(Brushes.Red, 0, 0, 480, 480);
            ThumbnailBitmap tb = new ThumbnailBitmap(b);

            t_image.Data = tb.Bitmap;
            t_image.Thumbnail = tb.Thumbnail;

            t_image.Id = Blog.CreateOrUpdatePostImage(Ticket, t_post.Id, t_image);
            Assert.Greater(t_image.Id, 0);

            Blog.DeletePost(Ticket, t_post.Id);
        }

        [Test]
        public void CreatePostWithNoTopicsTest()
        {
            // post
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);
            Blog.DeletePost(Ticket, t_post.Id);
        }

        [Test]
        public void CreatePostWithTwoTopicsTest()
        {
            // topic
            TransitTopic t_topic1 = new TransitTopic();
            t_topic1.Name = Guid.NewGuid().ToString();
            t_topic1.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic1);
            TransitTopic t_topic2 = new TransitTopic();
            t_topic2.Name = Guid.NewGuid().ToString();
            t_topic2.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic2);

            // post
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            List<TransitTopic> topics = new List<TransitTopic>();
            topics.Add(t_topic1);
            topics.Add(t_topic2);
            t_post.Topics = topics.ToArray();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            TransitPost t_post_retrieved = Blog.GetPostById(Ticket, t_post.Id);
            Assert.AreEqual(t_post_retrieved.Topics.Length, t_post.Topics.Length);

            Blog.DeletePost(Ticket, t_post.Id);
            Blog.DeleteTopic(Ticket, t_topic1.Id);
            Blog.DeleteTopic(Ticket, t_topic2.Id);
        }

        [Test]
        public void CreatePostWithANewTopicTest()
        {
            // topic that doesn't exist
            TransitTopic t_topic1 = new TransitTopic();
            t_topic1.Name = Guid.NewGuid().ToString();
            // topic that exists
            TransitTopic t_topic2 = new TransitTopic();
            t_topic2.Name = Guid.NewGuid().ToString();
            t_topic2.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic2);

            // post
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            List<TransitTopic> topics = new List<TransitTopic>();
            topics.Add(t_topic1);
            topics.Add(t_topic2);
            t_post.Topics = topics.ToArray();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            TransitPost t_post_retrieved = Blog.GetPostById(Ticket, t_post.Id);
            Assert.AreEqual(t_post_retrieved.Topics.Length, t_post.Topics.Length);

            Blog.DeletePost(Ticket, t_post.Id);
            t_topic1.Id = Blog.GetTopicByName(Ticket, t_topic1.Name).Id;
            Blog.DeleteTopic(Ticket, t_topic2.Id);
        }

        [Test]
        public void CreatePostWithImageAndCommentTest()
        {
            // post
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            // image
            TransitImage t_image = new TransitImage();
            t_image.Name = Guid.NewGuid().ToString();

            Bitmap b = new Bitmap(480, 480);
            Graphics g = Graphics.FromImage(b);
            g.FillEllipse(Brushes.Red, 0, 0, 480, 480);
            ThumbnailBitmap tb = new ThumbnailBitmap(b);

            t_image.Data = tb.Bitmap;
            t_image.Thumbnail = tb.Thumbnail;

            t_image.Id = Blog.CreateOrUpdatePostImage(Ticket, t_post.Id, t_image);
            Assert.Greater(t_image.Id, 0);

            // comment
            TransitComment t_comment = new TransitComment();
            t_comment.IpAddress = "127.0.0.1";
            t_comment.LoginId = Blog.GetLogin(Ticket).Id;
            t_comment.Text = Guid.NewGuid().ToString();

            t_comment.Id = Blog.CreateOrUpdateImageComment(Ticket, t_image.Id, t_comment);
            Assert.Greater(t_comment.Id, 0);

            Blog.DeleteImage(Ticket, t_image.Id);
            Blog.DeletePost(Ticket, t_post.Id);
        }

        [Test]
        public void CreatePostWithCommentTest()
        {
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            TransitComment t_comment = new TransitComment();
            t_comment.IpAddress = "127.0.0.1";
            t_comment.LoginId = Blog.GetLogin(Ticket).Id;
            t_comment.Text = Guid.NewGuid().ToString();

            t_comment.Id = Blog.CreateOrUpdatePostComment(Ticket, t_post.Id, t_comment);
            Assert.Greater(t_comment.Id, 0);

            Blog.DeletePost(Ticket, t_post.Id);
        }

        [Test]
        public void CreatePostIncrementCounterTest()
        {
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.Publish = true;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            Assert.AreEqual(1, Blog.IncrementPostCounter(Ticket, t_post.Id), 
                "New post counter must be one after a single increment.");

            Blog.DeletePost(Ticket, t_post.Id);
        }

        [Test]
        public void CreateStickyPostTest()
        {
            TransitPost t_post1 = new TransitPost();
            t_post1.Body = Guid.NewGuid().ToString();
            t_post1.Title = Guid.NewGuid().ToString();
            t_post1.Publish = true;
            t_post1.Sticky = true;
            t_post1.Created = t_post1.Modified = DateTime.UtcNow;
            t_post1.Id = Blog.CreateOrUpdatePost(Ticket, t_post1);
            Assert.Greater(t_post1.Id, 0);

            Thread.Sleep(1000);

            TransitPost t_post2 = new TransitPost();
            t_post2.Body = Guid.NewGuid().ToString();
            t_post2.Title = Guid.NewGuid().ToString();
            t_post2.Publish = true;
            t_post2.Sticky = false;
            t_post2.Created = t_post2.Modified = DateTime.UtcNow;
            t_post2.Id = Blog.CreateOrUpdatePost(Ticket, t_post2);
            Assert.Greater(t_post1.Id, 0);

            TransitPostQueryOptions queryOptions = new TransitPostQueryOptions();
            queryOptions.PageNumber = 0;
            queryOptions.PageSize = 2;
            queryOptions.DateStart = DateTime.MinValue;
            queryOptions.DateEnd = DateTime.MaxValue;
            queryOptions.SortDirection = WebServiceQuerySortDirection.Descending;
            queryOptions.SortExpression = "Created";
            TransitPost[] posts = Blog.GetPosts(Ticket, queryOptions);

            Blog.DeletePost(Ticket, t_post1.Id);
            Blog.DeletePost(Ticket, t_post2.Id);

            Assert.AreEqual(2, posts.Length);
            // make sure the sticky post is on top (the second post might not be in second position if there're other stick posts)
            Assert.AreEqual(t_post1.Id, posts[0].Id);
        }
    }
}
