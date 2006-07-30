using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Net;
using System.Web;
using System.IO;

namespace DBlog.TransitData
{
    public abstract class ManagedFeed
    {
        protected Feed mFeed = null;
        private XmlDocument mXmlFeed = null;

        public ManagedFeed(Feed feed)
        {
            mFeed = feed;
        }

        public bool NeedsUpdate
        {
            get
            {
                return (mFeed.Updated.AddSeconds(mFeed.Interval) < DateTime.UtcNow);
            }
        }

        protected abstract int UpdateFeedItems(ISession session);
        public abstract void DeleteFeedItems(ISession session);

        public XmlDocument XmlFeed
        {
            get
            {
                if (mXmlFeed == null)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mFeed.Url);
                    System.Net.ServicePointManager.Expect100Continue = false;
                    request.UserAgent = "DBlog.NET/2.0";
                    request.Timeout = 5000;
                    request.KeepAlive = false;
                    request.MaximumAutomaticRedirections = 5;

                    if (! string.IsNullOrEmpty(mFeed.Username))
                    {
                        request.Credentials = new NetworkCredential(
                            mFeed.Username, mFeed.Password, null);
                    }

                    mXmlFeed = new XmlDocument();
                    mXmlFeed.Load(new StreamReader(request.GetResponse().GetResponseStream()));
                }

                return mXmlFeed;
            }
        }

        public int Update(ISession session)
        {
            int result = 0;
            try
            {
                mFeed.Updated = DateTime.UtcNow;
                result = UpdateFeedItems(session);
                mFeed.Saved = DateTime.UtcNow;
                mFeed.Exception = string.Empty;
                session.Flush();
            }
            catch (Exception ex)
            {
                mFeed.Exception = ex.Message;
                session.Save(mFeed);
                session.Flush();
                throw;
            }

            return result;
        }

        public static int Update(Feed feed, ISession session, bool checktimestamp)
        {
            int result = 0;
            switch ((TransitFeedType)Enum.Parse(typeof(TransitFeedType), feed.Type))
            {
                case TransitFeedType.Rss:
                    ManagedRssFeed m_rss_feed = new ManagedRssFeed(feed);
                    if (!checktimestamp || m_rss_feed.NeedsUpdate)
                    {
                        result = m_rss_feed.Update(session);
                    }
                    break;
                case TransitFeedType.Atom:
                    ManagedAtomFeed m_atom_feed = new ManagedAtomFeed(feed);
                    if (!checktimestamp || m_atom_feed.NeedsUpdate)
                    {
                        result = m_atom_feed.Update(session);
                    }
                    break;
                default:
                    throw new Exception("Unsupported feed type.");
            }
            return result;
        }
    }
}
