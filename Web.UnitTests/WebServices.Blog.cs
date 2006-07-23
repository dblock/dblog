using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace DBlog.Web.UnitTests.WebServices
{
    public class BlogTest
    {
        WebServices.Blog.Blog mBlog = null;
        private string mUrl = "http://localhost/DBlog/WebServices.Blog.asmx";
        private List<BlogTest> mDependents = new List<BlogTest>();

        public BlogTest()
        {

        }

        protected string Url
        {
            get
            {
                return mUrl;
            }
        }

        protected WebServices.Blog.Blog Blog
        {
            get
            {
                return mBlog;
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            if (mBlog == null)
            {
                mBlog = new WebServices.Blog.Blog();
                mBlog.Url = mUrl;
            }

            foreach (BlogTest test in mDependents)
            {
                test.mBlog = mBlog;
                test.SetUp();
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            foreach (BlogTest test in mDependents)
            {
                test.TearDown();
            }

            mBlog = null;
        }

        public void AddDependent(BlogTest test)
        {
            mDependents.Add(test);
        }
    }
}
