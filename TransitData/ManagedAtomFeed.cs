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
    public class ManagedAtomFeed : ManagedFeed 
    {
        public ManagedAtomFeed(Feed feed)
            : base(feed)
        {

        }

        private XmlNamespaceManager mAtomNamespaceManager = null;

        public string AtomNamespace
        {
            get
            {
                string atomns = (string) ConfigurationManager.AppSettings["atomns"];
                if (atomns == null) atomns = "http://www.w3.org/2005/Atom"; // "http://purl.org/atom/ns#"
                return atomns;
            }
        }

        public XmlNamespaceManager AtomNamespaceManager
        {
            get
            {
                if (mAtomNamespaceManager == null)
                {
                    mAtomNamespaceManager = new XmlNamespaceManager(XmlFeed.NameTable);
                    mAtomNamespaceManager.AddNamespace("atom", AtomNamespace);
                }
                return mAtomNamespaceManager;
            }
        }

        public string ServicePostUrl
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

        protected override int UpdateFeedItems(ISession session)
        {
            string urlServicePost = ServicePostUrl;

            IList posts = session.CreateCriteria(typeof(Post))
                .Add(Expression.Ge("Modified", mFeed.Saved))
                .AddOrder(Order.Desc("Modified"))
                .List();

            foreach(Post post in posts)
            {
                XmlDocument xmlEntry = new XmlDocument();
                xmlEntry.LoadXml(
                 "<?xml version=\"1.0\"?>" +
                 "<entry xmlns=\"" + AtomNamespace + "\">" +
                 "<title type=\"text/html\" />" +
                 "<created />" +
                 "<content type=\"text/html\" mode=\"escaped\" />" +
                 "</entry>");

                XmlNamespaceManager xmlnsmgr = new XmlNamespaceManager(xmlEntry.NameTable);
                xmlnsmgr.AddNamespace("ns", AtomNamespace);

                xmlEntry.SelectSingleNode("//ns:title", xmlnsmgr).InnerText = Renderer.RenderEx(post.Title);
                xmlEntry.SelectSingleNode("//ns:created", xmlnsmgr).InnerText = post.Modified.ToString("s");
                xmlEntry.SelectSingleNode("//ns:content", xmlnsmgr).InnerText =
                 ((post.Created != post.Modified) ? "<b>UPDATE: " + post.Modified.ToString() + "</b><br/>" : string.Empty) +
                 TransitPost.Render(session, post.Id, post.Body);

                HttpWebRequest FeedRequest = (HttpWebRequest)WebRequest.Create(urlServicePost);
                if (! string.IsNullOrEmpty(mFeed.Username))
                {
                    FeedRequest.Credentials = new NetworkCredential(mFeed.Username, mFeed.Password, null);
                }

                // from http://haacked.com/archive/2004/05/15/449.aspx
                System.Net.ServicePointManager.Expect100Continue = false;

                FeedRequest.AllowAutoRedirect = false;
                FeedRequest.Method = "POST";
                FeedRequest.UserAgent = "DBlog.NET/2.0";
                FeedRequest.ContentType = "application/x.atom+xml;charset=utf-8";
                FeedRequest.Timeout = 5000;
                FeedRequest.KeepAlive = false;
                FeedRequest.MaximumAutomaticRedirections = 5;

                MemoryStream utf8ms = new MemoryStream();
                XmlTextWriter utf8tw = new XmlTextWriter(utf8ms, new UTF8Encoding(false));
                xmlEntry.Save(utf8tw);

                byte[] byteData = utf8ms.ToArray();
                FeedRequest.ContentLength = byteData.Length;
                FeedRequest.GetRequestStream().Write(byteData, 0, byteData.Length);
                FeedRequest.GetRequestStream().Close();

                try
                {
                    StreamReader tx = new StreamReader(FeedRequest.GetResponse().GetResponseStream());
                    string s = tx.ReadToEnd();
                    mFeed.Saved = post.Modified;                    
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        StreamReader rx = new StreamReader(ex.Response.GetResponseStream());
                        string s = rx.ReadToEnd();
                        s += ("<br>" + urlServicePost);
                        throw new Exception(s);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            return posts.Count;
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
