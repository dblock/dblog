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
        public void SetUp()
        {
            mBlog = new WebServices.Blog.Blog();
            mBlog.Url = mUrl;
        }

        [TearDown]
        public void TearDown()
        {
            mBlog = null;
        }
    }
}
