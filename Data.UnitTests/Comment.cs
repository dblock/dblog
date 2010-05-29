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
    public class CommentTest : NHibernateCrudTest
    {
        private Comment mComment = null;

        public Comment Comment
        {
            get
            {
                return mComment;
            }
        }

        public CommentTest()
        {
            LoginTest login = new LoginTest();
            AddDependentObject(login);

            mComment = new Comment();
            mComment.IpAddress = "127.0.0.1";
            mComment.Created = mComment.Modified = DateTime.UtcNow;
            mComment.Text = Guid.NewGuid().ToString(); ;
            mComment.OwnerLogin = login.Login;
        }

        public override object Object
        {
            get 
            {
                return mComment;
            }
        }
    }
}
