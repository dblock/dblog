using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.UnitTests
{
    [TestFixture]
    public class GalleryImageTest : NHibernateCrudTest
    {
        private GalleryImage mGalleryImage = null;

        public GalleryImage GalleryImage
        {
            get
            {
                return mGalleryImage;
            }
        }

        public GalleryImageTest()
        {
            GalleryTest gallery = new GalleryTest();
            AddDependentObject(gallery);

            ImageTest image = new ImageTest();
            AddDependentObject(image);

            mGalleryImage = new GalleryImage();
            mGalleryImage.Gallery = gallery.Gallery;
            mGalleryImage.Image = image.Image;
        }

        public override object Object
        {
            get 
            {
                return mGalleryImage;
            }
        }
    }
}
