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
    public class EntryImageTest : NHibernateCrudTest
    {
        private EntryImage mEntryImage = null;

        public EntryImageTest()
        {
            EntryTest entry = new EntryTest();
            AddDependentObject(entry);

            ImageTest image = new ImageTest();
            AddDependentObject(image);

            mEntryImage = new EntryImage();
            mEntryImage.Image = image.Image;
            mEntryImage.Entry = entry.Entry;
        }

        public override object Object
        {
            get 
            {
                return mEntryImage;
            }
        }
    }
}
