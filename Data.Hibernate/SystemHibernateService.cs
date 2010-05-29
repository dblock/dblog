using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NHibernate;
using NHibernate.Criterion;
using System.Threading;

namespace DBlog.Data.Hibernate
{
    public abstract class HibernateService : SystemService
    {
        public HibernateService()
        {

        }

        public virtual int SleepInterval
        {
            get
            {
                return 50;
            }
        }

        public abstract void Run(ISession session);

        public override void  Run()
        {
            ISessionFactory Factory = Session.Configuration.BuildSessionFactory();

            while (!IsStopping)
            {
                try
                {
                    IDbConnection conn = GetNewConnection();
                    conn.Open();

                    ISession session = Factory.OpenSession(conn);

                    try
                    {
                        Run(session);
                    }
                    finally
                    {
                        conn.Close();
                        session.Close();
                    }
                }
                catch
                {

                }

                Thread.Sleep(1000 * SleepInterval);
            }
        }
    }
}