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
    public class ImageCounterTest : NHibernateCrudTest
    {
        private ImageCounter mImageCounter = null;

        public ImageCounterTest()
        {
            ImageTest image = new ImageTest();
            AddDependentObject(image);

            CounterTest counter = new CounterTest();
            AddDependentObject(counter);

            mImageCounter = new ImageCounter();
            mImageCounter.Counter = counter.Counter;
            mImageCounter.Image = image.Image;
        }

        public override object Object
        {
            get 
            {
                return mImageCounter;
            }
        }
    }
}
