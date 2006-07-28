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
    public class BlogImageTest : BlogCrudTest
    {
        private TransitImage mImage = null;

        public override bool IsServiceCount
        {
            get
            {
                return true;
            }
        }

        public BlogImageTest()
        {
            mImage = new TransitImage();
            mImage.Data = Encoding.Default.GetBytes(Guid.NewGuid().ToString());
            mImage.Name = Guid.NewGuid().ToString();
            mImage.Description = Guid.NewGuid().ToString();
            mImage.Path = Guid.NewGuid().ToString();
            mImage.Preferred = false;
            mImage.Thumbnail = Encoding.Default.GetBytes(Guid.NewGuid().ToString());
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mImage;
            }
            set
            {
                mImage = null;
            }
        }

        public override string ObjectType
        {
            get
            {
                return "Image";
            }
        }

        [Test]
        public void TestGetImageWithData()
        {
            int id = Create();
            try
            {
                TransitImage imagewithbitmap = Blog.GetImageWithBitmapById(Ticket, id);
                Console.WriteLine(string.Format("GetImageWithBitmapById: {0}", imagewithbitmap.Id));
                Assert.IsNotNull(imagewithbitmap.Data, "Image data is null.");
                Assert.IsNull(imagewithbitmap.Thumbnail, "Image thumbnail is not null.");

                TransitImage imagewiththumbnail = Blog.GetImageWithThumbnailById(Ticket, id);
                Console.WriteLine(string.Format("GetImageWithThumbnailById: {0}", imagewithbitmap.Id));
                Assert.IsNull(imagewiththumbnail.Data, "Image data is not null.");
                Assert.IsNotNull(imagewiththumbnail.Thumbnail, "Image thumbnail is null.");
            }
            finally
            {
                Delete();
            }
        }

        [Test]
        public void TestGetNonBlogImages()
        {
            int id = Create(); // at least one image
            try
            {
                TransitImageQueryOptions options = new TransitImageQueryOptions();
                options.ExcludeBlogImages = true;
                int count = Blog.GetImagesCount(Ticket, options);
                Assert.IsTrue(count > 0);
                Console.WriteLine(string.Format("GetImagesCount: {0}", count));
                
                TransitImage[] images = Blog.GetImages(Ticket, options);
                Assert.AreEqual(count, images.Length); 
                Console.WriteLine(string.Format("GetImages: {0} images", images.Length));
            }
            finally
            {
                Delete();
            }
        }
    }
}
