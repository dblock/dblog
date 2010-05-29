using System;
using System.Collections.Generic;
using System.Text;
using DBlog.TransitData;
using DBlog.Data;
using System.Xml;
using System.Configuration;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using System.Collections;
using DBlog.Tools.Web;
using System.Net;
using System.IO;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace DBlog.TransitData
{
    public class ManagedRssFeed : ManagedFeed
    {
        public ManagedRssFeed(Feed feed)
            : base(feed)
        {

        }

        protected override int UpdateFeedItems(ISession session)
        {
            int result = 0;

            if (! string.IsNullOrEmpty(mFeed.Xsl))
            {
                StringBuilder TransformedString = new StringBuilder();
                XslCompiledTransform FeedXsl = new XslCompiledTransform();
                FeedXsl.Load(new XmlTextReader(new StringReader(mFeed.Xsl)), null, null);
                StringWriter StringWriter = new StringWriter(TransformedString);
                XmlTextWriter TextWriter = new XmlTextWriter(StringWriter);
                FeedXsl.Transform(new XmlNodeReader(XmlFeed.DocumentElement), TextWriter);
                XmlFeed.LoadXml(TransformedString.ToString());
            }

            XmlNodeList FeedXmlItems = XmlFeed.SelectNodes("descendant::channel/item");

            List<FeedItem> updated = new List<FeedItem>();
            
            foreach (XmlNode XmlNodeItem in FeedXmlItems)
            {
                XmlNode xmlLink = XmlNodeItem.SelectSingleNode("link");
                string link = (xmlLink != null) ? xmlLink.InnerText : null;

                FeedItem current = null;

                if (!string.IsNullOrEmpty(link))
                {
                    for (int i = 0; i < mFeed.FeedItems.Count; i++)
                    {
                        FeedItem item = (FeedItem)mFeed.FeedItems[i];
                        if (item.Link == link)
                        {
                            current = item;
                            updated.Add(item);
                            mFeed.FeedItems.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (current == null)
                {
                    result++;
                    current = new FeedItem();
                    current.Feed = mFeed;
                    current.Link = link;
                }

                XmlNode xmlDescription = XmlNodeItem.SelectSingleNode("description");
                current.Description = (xmlDescription != null) ? xmlDescription.InnerText : null;

                XmlNode xmlTitle = XmlNodeItem.SelectSingleNode("title");
                current.Title = (xmlTitle != null) ? xmlTitle.InnerText : null;

                session.Save(current);
            }

            foreach (FeedItem item in mFeed.FeedItems)
            {
                session.Delete(item);
            }

            mFeed.FeedItems = updated;
            return result;
        }

        public override void DeleteFeedItems(ISession session)
        {
            session.Delete(string.Format(
                "FeedItem WHERE Feed.Id = {0}", mFeed.Id));
        }
    }
}
