using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

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

        private bool mCrawler;

        public bool Crawler
        {
            get
            {
                return mCrawler;
            }
            set
            {
                mCrawler = value;
            }
        }

        public TransitBrowser()
        {

        }

        public TransitBrowser(DBlog.Data.Browser o)
            : base(o.Id)
        {
            Name = o.Name;
            Crawler = o.Crawler;
        }

        public Browser GetBrowser(ISession session)
        {
            Browser browser = (Id != 0) ? (Browser)session.Load(typeof(Browser), Id) : new Browser();
            browser.Name = Name;
            browser.Crawler = Crawler;
            return browser;
        }
    }
}
