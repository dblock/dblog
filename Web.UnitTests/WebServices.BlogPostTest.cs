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

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogPostTest : BlogCrudTest
    {
        private TransitPost mPost = null;
        private BlogTopicTest mTopicTest = null;

        public override bool IsServiceCount
        {
            get
            {
                return true;
            }
        }

        public BlogPostTest()
        {
            mPost = new TransitPost();
            mPost.Title = Guid.NewGuid().ToString();
            mPost.Body = Guid.NewGuid().ToString();

            mTopicTest = new BlogTopicTest();
            AddDependent(mTopicTest);
        }

        public override int Create()
        {
            mPost.LoginId = Blog.GetLogin(Ticket).Id;
            mTopicTest.Create();
            mPost.TopicId = mTopicTest.TransitInstance.Id;
            return base.Create();
        }

        public override void Delete()
        {
            base.Delete();
            mTopicTest.Delete();
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
            TransitTopic t_topic = new TransitTopic();
            t_topic.Name = Guid.NewGuid().ToString();
            t_topic.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic);

            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.TopicId = t_topic.Id;
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
            Blog.DeleteTopic(Ticket, t_topic.Id);
        }

        [Test]
        public void CreatePostWithCommentTest()
        {
            TransitTopic t_topic = new TransitTopic();
            t_topic.Name = Guid.NewGuid().ToString();
            t_topic.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic);

            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.TopicId = t_topic.Id;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            TransitComment t_comment = new TransitComment();
            t_comment.IpAddress = "127.0.0.1";
            t_comment.LoginId = Blog.GetLogin(Ticket).Id;
            t_comment.Text = Guid.NewGuid().ToString();

            t_comment.Id = Blog.CreateOrUpdatePostComment(Ticket, t_post.Id, t_comment);
            Assert.Greater(t_comment.Id, 0);

            Blog.DeletePost(Ticket, t_post.Id);
            Blog.DeleteTopic(Ticket, t_topic.Id);
        }

        [Test]
        public void CreatePostIncrementCounterTest()
        {
            TransitTopic t_topic = new TransitTopic();
            t_topic.Name = Guid.NewGuid().ToString();
            t_topic.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic);

            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
            t_post.TopicId = t_topic.Id;
            t_post.Id = Blog.CreateOrUpdatePost(Ticket, t_post);
            Assert.Greater(t_post.Id, 0);

            Assert.AreEqual(1, Blog.IncrementPostCounter(Ticket, t_post.Id), 
                "New post counter must be one after a single increment.");

            Blog.DeletePost(Ticket, t_post.Id);
            Blog.DeleteTopic(Ticket, t_topic.Id);
        }
    }
}
