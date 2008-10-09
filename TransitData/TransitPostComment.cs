using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostCommentQueryOptions : TransitAssociatedCommentQueryOptions
    {
        public int PostId
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

        public TransitPostCommentQueryOptions()
            : base("Post")
        {
        }

        public TransitPostCommentQueryOptions(
            int id)
            : base("Post", id)
        {

        }

        public TransitPostCommentQueryOptions(
            int id,
            int pagesize,
            int pagenumber)
            : base("Post", id, pagesize, pagenumber)
        {

        }
    }


    public class TransitPostComment : TransitAssociatedComment
    {
        public int PostId
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

        public TransitPostComment()
            : base()
        {

        }

        public TransitPostComment(ISession session, DBlog.Data.PostComment o, string ticket)
            : base(session, o.Post.Id, o.Comment, TransitPost.GetAccess(session, o.Post, ticket))
        {

        }

        public TransitPostComment(ISession session, DBlog.Data.PostComment o)
            : base(session, o.Post.Id, o.Comment)
        {

        }

        public PostComment GetPostComment(ISession session)
        {
            PostComment ei = (Id != 0) ? (PostComment)session.Load(typeof(PostComment), Id) : new PostComment();
            ei.Comment = (Comment)session.Load(typeof(Comment), CommentId);
            ei.Post = (Post)session.Load(typeof(Post), PostId);
            return ei;
        }
    }
}
