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
    public class EntryTest : NHibernateCrudTest
    {
        private Entry mEntry = null;

        public Entry Entry
        {
            get
            {
                return mEntry;
            }
        }

        public EntryTest()
        {
            LoginTest login = new LoginTest();
            AddDependentObject(login);

            TemplateTest template = new TemplateTest();
            AddDependentObject(template);

            TopicTest topic = new TopicTest();
            AddDependentObject(topic);

            mEntry = new Entry();
            mEntry.OwnerLogin = login.Login;
            mEntry.Created = mEntry.Modified = DateTime.UtcNow;
            mEntry.IpAddress = "127.0.0.1";
            mEntry.Template = template.Template;
            mEntry.Text = Guid.NewGuid().ToString();
            mEntry.Title = Guid.NewGuid().ToString();
            mEntry.Topic = topic.Topic;
        }

        public override object Object
        {
            get 
            {
                return mEntry;
            }
        }
    }
}
