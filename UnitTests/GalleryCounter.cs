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
    public class GalleryCounterTest : NHibernateCrudTest
    {
        private GalleryCounter mGalleryCounter = null;

        public GalleryCounter GalleryCounter
        {
            get
            {
                return mGalleryCounter;
            }
        }

        public GalleryCounterTest()
        {
            GalleryTest gallery = new GalleryTest();
            AddDependentObject(gallery);

            CounterTest counter = new CounterTest();
            AddDependentObject(counter);

            mGalleryCounter = new GalleryCounter();
            mGalleryCounter.Counter = counter.Counter;
            mGalleryCounter.Gallery = gallery.Gallery;
        }

        public override object Object
        {
            get 
            {
                return mGalleryCounter;
            }
        }

    }
}
