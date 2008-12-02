using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class HighlightTest : NHibernateCrudTest
    {
        private Highlight mHighlight = null;

        public Highlight Highlight
        {
            get
            {
                return mHighlight;
            }
        }

        public HighlightTest()
        {
            ImageTest image = new ImageTest();
            AddDependentObject(image);

            mHighlight = new Highlight();
            mHighlight.Description = Guid.NewGuid().ToString();
            mHighlight.Title = Guid.NewGuid().ToString();
            mHighlight.Url = Guid.NewGuid().ToString();
            mHighlight.Image = image.Image;
        }

        public override object Object
        {
            get 
            {
                return mHighlight;
            }
        }
    }
}
