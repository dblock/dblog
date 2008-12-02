using System;
using DBlog.Data;
using DBlog.Data.Hibernate.UnitTests;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using System.Collections.Generic;
using System.Text;

namespace DBlog.Data.UnitTests
{
    [TestFixture]
    public class ImageCommentTest : NHibernateCrudTest
    {
        private ImageComment mImageComment = null;

        public ImageCommentTest()
        {
            ImageTest image = new ImageTest();
            AddDependentObject(image);

            CommentTest comment = new CommentTest();
            AddDependentObject(comment);

            mImageComment = new ImageComment();
            mImageComment.Image = image.Image;
            mImageComment.Comment = comment.Comment;
        }

        public override object Object
        {
            get 
            {
                return mImageComment;
            }
        }
    }
}
