using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitComment : TransitObject
    {
        private int mParentCommentId = 0;

        public int ParentCommentId
        {
            get
            {
                return mParentCommentId;
            }
            set
            {
                mParentCommentId = value;
            }
        }

        private string mText;

        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
            }
        }

        private string mIpAddress;

        public string IpAddress
        {
            get
            {
                return mIpAddress;
            }
            set
            {
                mIpAddress = value;
            }
        }

        private DateTime mCreated;

        public DateTime Created
        {
            get
            {
                return mCreated;
            }
            set
            {
                mCreated = value;
            }
        }

        private DateTime mModified;

        public DateTime Modified
        {
            get
            {
                return mModified;
            }
            set
            {
                mModified = value;
            }
        }

        private int mLoginId;

        public int LoginId
        {
            get
            {
                return mLoginId;
            }
            set
            {
                mLoginId = value;
            }
        }

        private string mLoginName;

        public string LoginName
        {
            get
            {
                return mLoginName;
            }
            set
            {
                mLoginName = value;
            }
        }

        private string mLoginWebsite;

        public string LoginWebsite
        {
            get
            {
                return mLoginWebsite;
            }
            set
            {
                mLoginWebsite = value;
            }
        }

        public TransitComment()
        {

        }

        public TransitComment(ISession session, DBlog.Data.Comment o, string ticket)
            : this(session, o, HasAccess(session, o, ticket))
        {

        }

        public static bool HasAccess(ISession session, DBlog.Data.Comment comment, string ticket)
        {
            if (comment.PostComments == null || comment.PostComments.Count == 0)
                return true;

            foreach (PostComment pi in comment.PostComments)
            {
                if (TransitPost.GetAccess(session, pi.Post, ticket))
                    return true;
            }

            return false;
        }

        public TransitComment(ISession session, DBlog.Data.Comment o, bool hasaccess)
            : base(o.Id)
        {
            if (hasaccess)
            {
                Text = o.Text;
                IpAddress = o.IpAddress;
                LoginName = o.OwnerLogin.Name;
                LoginWebsite = o.OwnerLogin.Website;
            }

            LoginId = o.OwnerLogin.Id;

            ParentCommentId = (o.Threads != null && o.Threads.Count > 0) ?
                ((Thread) o.Threads[0]).ParentComment.Id : 0;
            
            Created = o.Created;
            Modified = o.Modified;
        }

        public Comment GetComment(ISession session)
        {
            Comment comment = (Id != 0) ? (Comment)session.Load(typeof(Comment), Id) : new Comment();
            comment.OwnerLogin = (LoginId != 0) ? (Login)session.Load(typeof(Login), LoginId) : null;
            comment.IpAddress = IpAddress;
            comment.Text = Text;
            return comment;
        }
    }
}
