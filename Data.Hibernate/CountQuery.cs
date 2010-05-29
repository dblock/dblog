using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace DBlog.Data.Hibernate
{
    public class CountQuery
    {
        private ISession mSession = null;
        private string mTable = string.Empty;
        private List<ICriterion> mExpressions = new List<ICriterion>();
        private Type mPersistentClass = null;
        private string mProperty = "Id";

        public string Table
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

        public CountQuery(ISession session, Type persistentclass, string table)
            : this(session, persistentclass, table, "Id")
        {
        }

        public CountQuery(ISession session, Type persistentclass, string table, string property)
        {
            mSession = session;
            mTable = table;
            mProperty = property;
            mPersistentClass = persistentclass;
        }

        public CountQuery Add(ICriterion item)
        {
            mExpressions.Add(item);
            return this;
        }

        public ICriteria CreateQuery()
        {
            ICriteria c = mSession.CreateCriteria(mPersistentClass);

            foreach (ICriterion expr in mExpressions)
            {
                c.Add(expr);
            }

            return c.SetProjection(Projections.Count(mProperty));
        }

        public T Execute<T>()
        {
            return CreateQuery().UniqueResult<T>();
        }
    }
}
