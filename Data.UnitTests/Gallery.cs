using System;
using DBlog.Data;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class GalleryTest : NHibernateCrudTest
    {
        private Gallery mGallery = null;
     
        public Gallery Gallery
        {
            get
            {
                return mGallery;
            }
        }

        public GalleryTest()
        {
            LoginTest login = new LoginTest();
            AddDependentObject(login);

            TopicTest topic = new TopicTest();
            AddDependentObject(topic);

            mGallery = new Gallery();
            mGallery.Created = mGallery.Modified = DateTime.UtcNow;
            mGallery.OwnerLogin = login.Login;
            mGallery.Path = Guid.NewGuid().ToString();
            mGallery.Text = Guid.NewGuid().ToString();
            mGallery.Title = Guid.NewGuid().ToString();
            mGallery.Topic = topic.Topic;
        }

        public override object Object
        {
            get 
            {
                return mGallery;
            }
        }
    }
}
