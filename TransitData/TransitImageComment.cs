using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitImageCommentQueryOptions : TransitAssociatedCommentQueryOptions
    {
        public int ImageId
        {
            get
            {
                return base.AssociatedId;
            }
            set
            {
                base.AssociatedId = value;
            }
        }

        public TransitImageCommentQueryOptions()
            : base("Image")
        {
        }

        public TransitImageCommentQueryOptions(
            int id) : base("Image", id)
        {

        }

        public TransitImageCommentQueryOptions(
            int id,
            int pagesize,
            int pagenumber)
            : base("Image", id, pagesize, pagenumber)
        {

        }
    }


    public class TransitImageComment : TransitAssociatedComment
    {
        public int ImageId
        {
            get
            {
                return base.AssociatedId;
            }
            set
            {
                base.AssociatedId = value;
            }
        }

        public TransitImageComment()
            : base()
        {
            AssociatedType = "Image";
        }

        public TransitImageComment(ISession session, DBlog.Data.ImageComment o, string ticket)
            : this(session, o, TransitImage.HasAccess(session, o.Image, ticket))
        {
            AssociatedType = "Image";
        }

        public TransitImageComment(ISession session, DBlog.Data.ImageComment o, bool hasaccess)
            : base(session, o.Image.Id, o.Comment, hasaccess)
        {
            AssociatedType = "Image";
        }

        public ImageComment GetImageComment(ISession session)
        {
            ImageComment ei = (Id != 0) ? (ImageComment)session.Load(typeof(ImageComment), Id) : new ImageComment();
            ei.Comment = (Comment)session.Load(typeof(Comment), CommentId);
            ei.Image = (Image)session.Load(typeof(Image), ImageId);
            return ei;
        }
    }
}
