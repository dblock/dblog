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
    public class EntryCommentTest : NHibernateCrudTest
    {
        private EntryComment mEntryComment = null;

        public EntryCommentTest()
        {
            CommentTest comment = new CommentTest();
            AddDependentObject(comment);

            EntryTest entry = new EntryTest();
            AddDependentObject(entry);

            mEntryComment = new EntryComment();
            mEntryComment.Comment = comment.Comment;
            mEntryComment.Entry = entry.Entry;
        }

        public override object Object
        {
            get 
            {
                return mEntryComment;
            }
        }
    }
}
