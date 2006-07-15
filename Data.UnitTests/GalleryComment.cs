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
    public class GalleryCommentTest : NHibernateCrudTest
    {
        private GalleryComment mGalleryComment = null;

        public GalleryComment GalleryComment
        {
            get
            {
                return mGalleryComment;
            }
        }

        public GalleryCommentTest()
        {
            GalleryTest gallery = new GalleryTest();
            AddDependentObject(gallery);

            CommentTest comment = new CommentTest();
            AddDependentObject(comment);

            mGalleryComment = new GalleryComment();
            mGalleryComment.Gallery = gallery.Gallery;
            mGalleryComment.Comment = comment.Comment;
        }

        public override object Object
        {
            get 
            {
                return mGalleryComment;
            }
        }
    }
}
