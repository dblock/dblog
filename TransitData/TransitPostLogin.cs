using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitPostLoginQueryOptions : WebServiceQueryOptions
    {
        private int mPostId = 0;

        public int PostId
        {
            get
            {
                return mPostId;
            }
            set
            {
                mPostId = value;
            }
        }

        public TransitPostLoginQueryOptions()
        {
        }

        public TransitPostLoginQueryOptions(
            int postid)
        {
            mPostId = postid;
        }

        public TransitPostLoginQueryOptions(
            int postid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mPostId = postid;
        }

        public override void Apply(ICriteria criteria)
        {
            if (PostId != 0)
            {
                criteria.Add(Expression.Eq("Post.Id", PostId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (PostId != 0)
            {
                query.Add(Expression.Eq("Post.Id", PostId));
            }

            base.Apply(query);
        }
    }


    public class TransitPostLogin : TransitObject
    {
        private TransitLogin mLogin = null;

        public TransitLogin Login
        {
            get
            {
                return mLogin;
            }
            set
            {
                mLogin = value;
            }
        }

        private TransitPost mPost = null;

        public TransitPost Post
        {
            get
            {
                return mPost;
            }
            set
            {
                mPost = value;
            }
        }

        public TransitPostLogin()
        {

        }

        public TransitPostLogin(ISession session, DBlog.Data.PostLogin o)
            : base(o.Id)
        {
            Post = new TransitPost(session, o.Post, false);
            Login = new TransitLogin(o.Login);
        }

        public PostLogin GetPostLogin(ISession session)
        {
            PostLogin ei = (Id != 0) ? (PostLogin)session.Load(typeof(PostLogin), Id) : new PostLogin();
            ei.Login = (Login)session.Load(typeof(Login), Login.Id);
            ei.Post = (Post)session.Load(typeof(Post), Post.Id);
            return ei;
        }
    }
}
