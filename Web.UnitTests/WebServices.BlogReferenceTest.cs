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
    public class BlogReferenceTest : BlogCrudTest
    {
        private TransitReference mReference = null;

        public BlogReferenceTest()
        {
            mReference = new TransitReference();
            mReference.Word = Guid.NewGuid().ToString();
            mReference.Result = Guid.NewGuid().ToString();
            mReference.Url = Guid.NewGuid().ToString();
        }

        public override TransitObject TransitInstance
        {
            get
            {
                return mReference;
            }
            set
            {
                mReference = null;
            }
        }

        public override string ObjectType 
        {
            get
            {
                return "Reference";
            }
        }

        [Test]
        public void TestSearchReferences()
        {
            int id = Create();
            Console.WriteLine("Reference: {0}", id);
            int count = Blog.SearchReferencesCount(Ticket, mReference.Word);
            Console.WriteLine("Count: {0}", count);
            Assert.IsTrue(count > 0, "Invalid count, no references returned.");
            TransitReference[] refs = Blog.SearchReferences(Ticket, mReference.Word, null);
            Assert.IsNotNull(refs, "Invalid references returned.");
            Assert.AreEqual(refs.Length, count);
            Delete();
        }
    }
}
