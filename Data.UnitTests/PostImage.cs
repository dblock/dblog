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
    public class PostImageTest : NHibernateCrudTest
    {
        private PostImage mPostImage = null;

        public PostImageTest()
        {
            PostTest Post = new PostTest();
            AddDependentObject(Post);

            ImageTest image = new ImageTest();
            AddDependentObject(image);

            mPostImage = new PostImage();
            mPostImage.Image = image.Image;
            mPostImage.Post = Post.Post;
        }

        public override object Object
        {
            get 
            {
                return mPostImage;
            }
        }
    }
}
