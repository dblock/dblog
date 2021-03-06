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
    public class PostCommentTest : NHibernateCrudTest
    {
        private PostComment mPostComment = null;

        public PostCommentTest()
        {
            CommentTest comment = new CommentTest();
            AddDependentObject(comment);

            PostTest Post = new PostTest();
            AddDependentObject(Post);

            mPostComment = new PostComment();
            mPostComment.Comment = comment.Comment;
            mPostComment.Post = Post.Post;
        }

        public override object Object
        {
            get 
            {
                return mPostComment;
            }
        }
    }
}
