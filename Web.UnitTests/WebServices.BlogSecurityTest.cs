using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DBlog.Tools;
using DBlog.Web.UnitTests.WebServices.Blog;
using NUnit;
using System.Drawing;
using DBlog.Tools.Drawing;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogSecurityTest : BlogTest
    {
        public BlogSecurityTest()
        {

        }

        [Test]
        public void CreateSecurePostTest()
        {
            TransitPost t_post = new TransitPost();
            t_post.Body = Guid.NewGuid().ToString();
            t_post.Title = Guid.NewGuid().ToString();
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

            TransitLogin t_login = new TransitLogin();
            t_login.Username = Guid.NewGuid().ToString();
            t_login.Password = Guid.NewGuid().ToString();
            t_login.Role = TransitLoginRole.Guest;
            t_login.Id = Blog.CreateOrUpdateLogin(Ticket, t_login);
            Assert.Greater(t_login.Id, 0);

            TransitComment t_comment = new TransitComment();
            t_comment.IpAddress = "127.0.0.1";
            t_comment.Text = Guid.NewGuid().ToString();
            t_comment.LoginId = t_login.Id;
            t_comment.Id = Blog.CreateOrUpdatePostComment(Ticket, t_post.Id, t_comment);
            Assert.Greater(t_comment.Id, 0);

            int t_postlogin_id = Blog.CreateOrUpdatePostLogin(Ticket, t_post.Id, t_login);
            Assert.Greater(t_postlogin_id, 0);

            string authticket = Blog.Login(t_login.Username, t_login.Password);
            
            // check access to posts

            TransitPost t_post_unauthorized = Blog.GetPostById(null, t_post.Id);
            Assert.IsTrue(string.IsNullOrEmpty(t_post_unauthorized.Body), "Unathorized post body wasn't stripped.");

            TransitPost t_post_authorized = Blog.GetPostById(authticket, t_post.Id);
            Assert.IsFalse(string.IsNullOrEmpty(t_post_authorized.Body), "Authorized post was stripped.");

            // check access to images

            TransitImage t_image_unauthorized = Blog.GetImageWithBitmapById(null, t_image.Id);
            Assert.IsTrue(t_image_unauthorized.Data == null, "Unathorized image returned data.");

            TransitImage t_image_authorized = Blog.GetImageWithBitmapById(authticket, t_image.Id);
            Assert.IsTrue(t_image_authorized.Data != null, "Authorized image didn't return data.");

            // check access to comments

            TransitComment t_comment_unauthorized = Blog.GetCommentById(null, t_comment.Id);
            Assert.IsTrue(string.IsNullOrEmpty(t_comment_unauthorized.Text), "Unathorized comment returned data.");

            TransitComment t_comment_authorized = Blog.GetCommentById(authticket, t_comment.Id);
            Assert.IsFalse(string.IsNullOrEmpty(t_comment_authorized.Text), "Authorized comment didn't return data.");

            Blog.DeletePost(Ticket, t_post.Id);
        }

    }
}
