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
            TemplateTest template = new TemplateTest();
            AddDependentObject(template);

            LoginTest login = new LoginTest();
            AddDependentObject(login);

            mComment = new Comment();
            mComment.IpAddress = "127.0.0.1";
            mComment.Created = mComment.Modified = DateTime.UtcNow;
            mComment.Text = Guid.NewGuid().ToString(); ;
            mComment.Template = template.Template;
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
