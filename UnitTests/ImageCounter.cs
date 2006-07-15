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
