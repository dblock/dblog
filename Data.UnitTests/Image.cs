using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class ImageTest : NHibernateCrudTest
    {
        private Image mImage = null;

        public Image Image
        {
            get
            {
                return mImage;
            }
        }

        public ImageTest()
        {
            mImage = new Image();
            mImage.Data = Encoding.Default.GetBytes(Guid.NewGuid().ToString());
            mImage.Description = Guid.NewGuid().ToString();
            mImage.Modified = DateTime.UtcNow;
            mImage.Name = Guid.NewGuid().ToString();
            mImage.Path = Guid.NewGuid().ToString();
            mImage.Preferred = false;
            mImage.Thumbnail = Encoding.Default.GetBytes(Guid.NewGuid().ToString());
        }

        public override object Object
        {
            get 
            {
                return mImage;
            }
        }
    }
}
