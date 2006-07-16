using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Expression;
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
        {
            mSession = session;
            mTable = table;
            mPersistentClass = persistentclass;
        }

        public CountQuery Add(ICriterion item)
        {
            mExpressions.Add(item);
            return this;
        }

        public IQuery CreateQuery()
        {
            return mSession.CreateQuery(this.ToString());
        }

        public int execute() // consistent with the naming of NHibernate
        {
            return (int) CreateQuery().UniqueResult();
        }

        public override string ToString()
        {
            ISessionFactoryImplementor impl = (ISessionFactoryImplementor) mSession.SessionFactory;

            StringBuilder s = new StringBuilder();
            s.AppendFormat("SELECT COUNT({1}) FROM {0} {1}", Table, Table);

            Dictionary<string, string> aliasclasses = new Dictionary<string,string>();

            StringBuilder q = new StringBuilder();
            foreach (ICriterion expr in mExpressions)
            {
                q.Append((q.Length > 0) ? " AND " : " WHERE ");
                SqlString ss = expr.ToSqlString(impl, mPersistentClass, Table, aliasclasses);

                TypedValue[] values = expr.GetTypedValues(impl, mPersistentClass, aliasclasses);

                int index = 0;
                int[] pi = ss.ParameterIndexes;
                foreach (int i in pi)
                {
                    string v = values[index].Value.ToString();
                    v = v.Replace("'", "''");
                    ss.SqlParts[i] = string.Format("'{0}'", v);
                    index++;
                }
                string ss_s = ss.ToString();
                ss_s = ss_s.Replace("_", "."); // hack
                q.Append(ss_s);
            }

            s.Append(q);
           return s.ToString();
        }
    }
}
