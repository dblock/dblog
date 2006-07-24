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
    public class BlogEntryTest : BlogCrudTest
    {
        private TransitEntry mEntry = null;
        private BlogTopicTest mTopicTest = null;

        public BlogEntryTest()
        {
            mEntry = new TransitEntry();
            mEntry.Title = Guid.NewGuid().ToString();
            mEntry.Text = Guid.NewGuid().ToString();
            mEntry.IpAddress = "127.0.0.1";

            mTopicTest = new BlogTopicTest();
            AddDependent(mTopicTest);
        }

        public override int Create()
        {
            mEntry.OwnerLoginId = Blog.GetLogin(Ticket).Id;
            mTopicTest.Create();
            mEntry.TopicId = mTopicTest.TransitInstance.Id;
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
                return "Entries";
            }
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mEntry;
            }
            set
            {
                mEntry = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Entry";
            }
        }

        [Test]
        public void CreateEntryWithImageTest()
        {
            TransitTopic t_topic = new TransitTopic();
            t_topic.Name = Guid.NewGuid().ToString();
            t_topic.Id = Blog.CreateOrUpdateTopic(Ticket, t_topic);

            TransitEntry t_entry = new TransitEntry();
            t_entry.Text = Guid.NewGuid().ToString();
            t_entry.Title = Guid.NewGuid().ToString();
            t_entry.IpAddress = "127.0.0.1";
            t_entry.TopicId = t_topic.Id;
            t_entry.Id = Blog.CreateOrUpdateEntry(Ticket, t_entry);
            Assert.Greater(t_entry.Id, 0);

            TransitImage t_image = new TransitImage();
            t_image.Name = Guid.NewGuid().ToString();
            
            Bitmap b = new Bitmap(480, 480);
            Graphics g = Graphics.FromImage(b);
            g.FillEllipse(Brushes.Red, 0, 0, 480, 480);
            ThumbnailBitmap tb = new ThumbnailBitmap(b);

            t_image.Data = tb.Bitmap;
            t_image.Thumbnail = tb.Thumbnail;

            t_image.Id = Blog.CreateOrUpdateEntryImage(Ticket, t_entry.Id, t_image);
            Assert.Greater(t_image.Id, 0);

            Blog.DeleteEntry(Ticket, t_entry.Id);
        }
    }
}
