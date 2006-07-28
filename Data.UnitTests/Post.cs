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
    public class PostTest : NHibernateCrudTest
    {
        private Post mPost = null;

        public Post Post
        {
            get
            {
                return mPost;
            }
        }

        public PostTest()
        {
            LoginTest login = new LoginTest();
            AddDependentObject(login);

            TopicTest topic = new TopicTest();
            AddDependentObject(topic);

            mPost = new Post();
            mPost.Login = login.Login;
            mPost.Created = mPost.Modified = DateTime.UtcNow;
            mPost.Body = Guid.NewGuid().ToString();
            mPost.Title = Guid.NewGuid().ToString();
            mPost.Topic = topic.Topic;
        }

        public override object Object
        {
            get
            {
                return mPost;
            }
        }
    }
}
