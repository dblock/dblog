using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public class TransitBrowserVersion : TransitObject
    {
        private int mMajor;

        public int Major
        {
            get
            {
                return mMajor;
            }
            set
            {
                mMajor = value;
            }
        }

        private int mMinor;

        public int Minor
        {
            get
            {
                return mMinor;
            }
            set
            {
                mMinor = value;
            }
        }

        private TransitBrowser mBrowser;

        public TransitBrowser Browser
        {
            get
            {
                return mBrowser;
            }
            set
            {
                mBrowser = value;
            }
        }

        public TransitBrowserVersion()
        {

        }

        public TransitBrowserVersion(DBlog.Data.BrowserVersion o)
            : base(o.Id)
        {
            Major = o.Major;
            Minor = o.Minor;
            Browser = new TransitBrowser(o.Browser);
        }

        public BrowserVersion GetBrowserVersion(ISession session)
        {
            BrowserVersion browserversion = (Id != 0) ? (BrowserVersion)session.Load(typeof(BrowserVersion), Id) : new BrowserVersion();
            browserversion.Browser = (Browser != null && Browser.Id > 0) ? (Browser) session.Load(typeof(Browser), Browser.Id) : null;
            browserversion.Major = Major;
            browserversion.Minor = Minor;
            return browserversion;
        }
    }
}
