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
    public class PostLoginTest : NHibernateCrudTest
    {
        private PostLogin mPostLogin = null;

        public PostLogin PostLogin
        {
            get
            {
                return mPostLogin;
            }
        }

        public PostLoginTest()
        {
            PostTest Post = new PostTest();
            AddDependentObject(Post);

            LoginTest login = new LoginTest();
            AddDependentObject(login);

            mPostLogin = new PostLogin();
            mPostLogin.Post = Post.Post;
            mPostLogin.Login = login.Login;
        }

        public override object Object
        {
            get 
            {
                return mPostLogin;
            }
        }
    }
}
