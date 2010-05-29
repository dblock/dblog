using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Criterion;

namespace DBlog.TransitData
{
    public class TransitBrowser : TransitObject
    {
        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        private string mVersion;

        public string Version
        {
            get
            {
                return mVersion;
            }
            set
            {
                mVersion = value;
            }
        }

        private string mPlatform;

        public string Platform
        {
            get
            {
                return mPlatform;
            }
            set
            {
                mPlatform = value;
            }
        }

        public TransitBrowser()
        {

        }

        public TransitBrowser(DBlog.Data.Browser o)
            : base(o.Id)
        {
            Name = o.Name;
            Version = o.Version;
            Platform = o.Platform;
        }

        public Browser GetBrowser(ISession session)
        {
            Browser browser = null;
            if (Id == 0)
            {
                browser = (Browser)session.CreateCriteria(typeof(Browser))
                    .Add(Expression.Eq("Name", Name))
                    .Add(Expression.Eq("Platform", Platform))
                    .Add(Expression.Eq("Version", Version))
                    .UniqueResult();

                if (browser == null)
                {
                    browser = new Browser();
                }
            }
            else
            {
                browser = (Browser) session.Load(typeof(Browser), Id);
            }

            browser.Name = Name;
            browser.Version = Version;
            browser.Platform = Platform;

            return browser;
        }
    }
}
