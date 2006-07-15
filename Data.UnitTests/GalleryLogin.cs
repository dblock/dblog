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
    public class GalleryLoginTest : NHibernateCrudTest
    {
        private GalleryLogin mGalleryLogin = null;

        public GalleryLogin GalleryLogin
        {
            get
            {
                return mGalleryLogin;
            }
        }

        public GalleryLoginTest()
        {
            GalleryTest gallery = new GalleryTest();
            AddDependentObject(gallery);

            LoginTest login = new LoginTest();
            AddDependentObject(login);

            mGalleryLogin = new GalleryLogin();
            mGalleryLogin.Gallery = gallery.Gallery;
            mGalleryLogin.Login = login.Login;
        }

        public override object Object
        {
            get 
            {
                return mGalleryLogin;
            }
        }
    }
}
