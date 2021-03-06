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
    public class ThreadTest : NHibernateCrudTest
    {
        private Thread mThread = null;

        public ThreadTest()
        {
            CommentTest comment = new CommentTest();
            AddDependentObject(comment);

            CommentTest parentcomment = new CommentTest();
            AddDependentObject(parentcomment);

            mThread = new Thread();
            mThread.Comment = comment.Comment;
            mThread.ParentComment = parentcomment.Comment;
        }

        public override object Object
        {
            get 
            {
                return mThread;
            }
        }
    }
}
