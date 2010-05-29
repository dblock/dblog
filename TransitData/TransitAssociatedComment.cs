using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitAssociatedCommentQueryOptions : WebServiceQueryOptions
    {
        private string mTable;

        private int mAssociatedId;

        public int AssociatedId
        {
            get
            {
                return mAssociatedId;
            }
            set
            {
                mAssociatedId = value;
            }
        }

        private string Table
        {
            get
            {
                return mTable;
            }
            set
            {
                mTable = value;
            }
        }

        public TransitAssociatedCommentQueryOptions(string table)
        {
            mTable = table;
        }

        public TransitAssociatedCommentQueryOptions(
            string table,
            int id)
        {
            mTable = table;
            AssociatedId = id;
        }

        public TransitAssociatedCommentQueryOptions(
            string table,
            int id,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mTable = table;
            AssociatedId = id;
        }

        public override void Apply(ICriteria criteria)
        {
            if (AssociatedId != 0)
            {
                criteria.Add(Expression.Eq(string.Format("{0}.Id", Table), AssociatedId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (AssociatedId != 0)
            {
                query.Add(Expression.Eq(string.Format("{0}.Id", Table), AssociatedId));
            }

            base.Apply(query);
        }
    }

    public class TransitAssociatedComment : TransitObject
    {
        private int mCommentId;

        public int CommentId
        {
            get
            {
                return mCommentId;
            }
            set
            {
                mCommentId = value;
            }
        }

        private int mAssociatedId;

        public int AssociatedId
        {
            get
            {
                return mAssociatedId;
            }
            set
            {
                mAssociatedId = value;
            }
        }

        private string mCommentText;

        public string CommentText
        {
            get
            {
                return mCommentText;
            }
            set
            {
                mCommentText = value;
            }
        }

        private string mCommentIpAddress;

        public string CommentIpAddress
        {
            get
            {
                return mCommentIpAddress;
            }
            set
            {
                mCommentIpAddress = value;
            }
        }

        private string mCommentLoginName;

        public string CommentLoginName
        {
            get
            {
                return mCommentLoginName;
            }
            set
            {
                mCommentLoginName = value;
            }
        }

        private string mCommentLoginWebsite;

        public string CommentLoginWebsite
        {
            get
            {
                return mCommentLoginWebsite;
            }
            set
            {
                mCommentLoginWebsite = value;
            }
        }

        private DateTime mCommentCreated;

        public DateTime CommentCreated
        {
            get
            {
                return mCommentCreated;
            }
            set
            {
                mCommentCreated = value;
            }
        }

        private DateTime mCommentModified;

        public DateTime CommentModified
        {
            get
            {
                return mCommentModified;
            }
            set
            {
                mCommentModified = value;
            }
        }

        private int mCommentLevel = 0;

        public int CommentLevel
        {
            get
            {
                return mCommentLevel;
            }
            set
            {
                mCommentLevel = value;
            }
        }

        private string mAssociatedType = string.Empty;
        public string AssociatedType
        {
            get
            {
                return mAssociatedType;
            }
            set
            {
                mAssociatedType = value;
            }
        }
            
        public TransitAssociatedComment()
        {

        }

        public TransitAssociatedComment(ISession session, int id, DBlog.Data.Comment o)
            : this(session, id, o, true)
        {
        }

        public TransitAssociatedComment(ISession session, DBlog.Data.Hibernate.AssociatedComment o)
            : this(session, o, true)
        {

        }

        public TransitAssociatedComment(ISession session, DBlog.Data.Hibernate.AssociatedComment o, string ticket)
            : this(session, o, ManagedLogin.IsAdministrator(session, ticket))
        {

        }

        public TransitAssociatedComment(ISession session, DBlog.Data.Hibernate.AssociatedComment o, bool hasaccess)
        {
            CommentId = o.Id;
            AssociatedId = o.AssociatedId;
            AssociatedType = o.Type;

            if (hasaccess)
            {
                CommentIpAddress = o.IpAddress;
                CommentText = o.Text;
                CommentCreated = o.Created;
                CommentModified = o.Modified;
                CommentLevel = 0;
            }
        }

        public TransitAssociatedComment(ISession session, int id, DBlog.Data.Comment o, bool hasaccess)
            : base(id)
        {
            AssociatedId = id;
            CommentId = o.Id;

            if (hasaccess)
            {
                CommentIpAddress = o.IpAddress;
                CommentText = o.Text;
                if (o.OwnerLogin != null)
                {
                    CommentLoginName = o.OwnerLogin.Name;
                    CommentLoginWebsite = o.OwnerLogin.Website;
                }
                CommentCreated = o.Created;
                CommentModified = o.Modified;
                CommentLevel = GetLevel(o);
            }
        }

        public static int GetLevel(Comment comment)
        {
            int result = 0;
            do
            {
                if (comment.Threads == null)
                    break;

                if (comment.Threads.Count == 0)
                    break;

                result++;

                comment = ((Thread)comment.Threads[0]).ParentComment;
            }
            while (comment != null);

            return result;
        }
    }
}
