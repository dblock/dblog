using System;
using System.Collections.Generic;
using System.Text;
using DBlog.TransitData;
using DBlog.Data;
using System.Xml;
using System.Configuration;
using System.Web;
using NHibernate;
using NHibernate.Expression;
using System.Collections;
using DBlog.Tools.Web;
using System.Net;
using System.IO;

namespace DBlog.TransitData
{
    public class ManagedAtomFeed : ManagedAtomPostFeed 
    {
        public ManagedAtomFeed(Feed feed)
            : base(feed)
        {

        }

        public override string ServicePostUrl
        {
            get
            {
                XmlNode xmlServicePost = XmlFeed.SelectSingleNode("/atom:feed/atom:link[@rel='service.post']", AtomNamespaceManager);
                if (xmlServicePost == null)
                    xmlServicePost = XmlFeed.SelectSingleNode("/atom:feed/atom:link[@rel='service.post']", AtomNamespaceManager);
                if (xmlServicePost == null)
                    throw new Exception("Feed does not contain a /feed/link[rel='service.post'] node in '" + AtomNamespace + "'.");
                XmlAttribute xmlHref = xmlServicePost.Attributes["href"];
                if (xmlHref == null)
                    throw new Exception("Service.post node is missing an href attribute.");
                return xmlHref.InnerText;
            }
        }

        public override void DeleteFeedItems(ISession session)
        {
            XmlNodeList FeedXmlItems = XmlFeed.SelectNodes("/atom:feed/atom:entry", AtomNamespaceManager);

            foreach (XmlNode XmlNodeItem in FeedXmlItems)
            {
                XmlNode xmlLink = XmlNodeItem.SelectSingleNode("atom:link[@rel='service.edit']", AtomNamespaceManager);

                if (xmlLink == null)
                    throw new Exception("Missing /feed/entry/link[@rel='service.edit' in '" + AtomNamespace + "'.");

                XmlAttribute xmlHref = xmlLink.Attributes["href"];

                if (xmlHref == null)
                    throw new Exception("Element /feed/entry/link[@rel='service.edit'] is missing an href attribute.");

                HttpWebRequest DeleteRequest = (HttpWebRequest)WebRequest.Create(xmlHref.InnerText);
                if (! string.IsNullOrEmpty(mFeed.Username))
                {
                    DeleteRequest.Credentials = new NetworkCredential(mFeed.Username, mFeed.Password, null);
                }

                DeleteRequest.Method = "DELETE";
                DeleteRequest.UserAgent = "DBlog.NET/1.0";

                string rx = new StreamReader(DeleteRequest.GetResponse().GetResponseStream()).ReadToEnd();
            }

        }
    }
}
