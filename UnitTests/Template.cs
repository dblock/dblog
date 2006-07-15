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
    public class TemplateTest : NHibernateCrudTest
    {
        private Template mTemplate = null;

        public Template Template
        {
            get
            {
                return mTemplate;
            }
        }

        public TemplateTest()
        {
            mTemplate = new Template();

            mTemplate.Name = Guid.NewGuid().ToString();
            mTemplate.Source = Guid.NewGuid().ToString();
            mTemplate.Type = Guid.NewGuid().ToString();
        }

        public override object Object
        {
            get 
            {
                return mTemplate;
            }
        }
    }
}
