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
                    try
                    {
                        mXmlFeed.Load(new StreamReader(request.GetResponse().GetResponseStream()));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error loading {0}: {1}",
                            mFeed.Url, ex.Message), ex);
                    }
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
                    ManagedRssFeed rssFeed = new ManagedRssFeed(feed);
                    if (!checktimestamp || rssFeed.NeedsUpdate)
                    {
                        result = rssFeed.Update(session);
                    }
                    break;
                case TransitFeedType.Atom:
                    ManagedAtomFeed atomFeed = new ManagedAtomFeed(feed);
                    if (!checktimestamp || atomFeed.NeedsUpdate)
                    {
                        result = atomFeed.Update(session);
                    }
                    break;
                case TransitFeedType.AtomPost:
                    ManagedAtomPostFeed atomPostFeed = new ManagedAtomPostFeed(feed);
                    if (!checktimestamp || atomPostFeed.NeedsUpdate)
                    {
                        result = atomPostFeed.Update(session);
                    }
                    break;
                case TransitFeedType.ZenFlashGallery:
                    ManagedZenFlashGalleryFeed zenFeed = new ManagedZenFlashGalleryFeed(feed);
                    if (!checktimestamp || zenFeed.NeedsUpdate)
                    {
                        result = zenFeed.Update(session);
                    }
                    break;
                default:
                    throw new Exception("Unsupported feed type.");
            }
            return result;
        }
    }
}
