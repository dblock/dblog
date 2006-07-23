using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Reflection;

namespace DBlog.Web.UnitTests.WebServices
{
    [TestFixture]
    public class BlogHighlightTest : BlogCrudTest
    {
        private TransitHighlight mHighlight = null;
        private BlogImageTest mImageTest = null;

        public BlogHighlightTest()
        {
            mHighlight = new TransitHighlight();
            mHighlight.Title = Guid.NewGuid().ToString();
            mHighlight.Description = Guid.NewGuid().ToString();
            mHighlight.Url = Guid.NewGuid().ToString();

            mImageTest = new BlogImageTest();
            AddDependent(mImageTest);
        }

        public override int Create()
        {
            mHighlight.ImageId = mImageTest.Create();
            return base.Create();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mHighlight;
            }
            set
            {
                mHighlight = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "Highlight";
            }
        }

        public override void Delete()
        {
            base.Delete();
            mImageTest.Delete();
        }
    }
}
