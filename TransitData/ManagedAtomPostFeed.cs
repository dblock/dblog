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
    public class ManagedAtomPostFeed : ManagedFeed
    {
        public ManagedAtomPostFeed(Feed feed)
            : base(feed)
        {

        }

        private XmlNamespaceManager mAtomNamespaceManager = null;

        public string AtomNamespace
        {
            get
            {
                string atomns = (string)ConfigurationManager.AppSettings["atomns"];
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

        public virtual string ServicePostUrl
        {
            get
            {
                return mFeed.Url;
            }
        }

        protected override int UpdateFeedItems(ISession session)
        {
            string urlServicePost = ServicePostUrl;

            IList posts = session.CreateCriteria(typeof(Post))
                .Add(Expression.Ge("Modified", mFeed.Saved))
                .Add(Expression.Eq("Publish", true))
                .Add(Expression.Eq("Display", true))
                .AddOrder(Order.Desc("Modified"))
                .List();

            foreach (Post post in posts)
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

                xmlEntry.SelectSingleNode("//ns:title", xmlnsmgr).InnerText = Renderer.RenderEx(post.Title, ConfigurationManager.AppSettings["url"], null);
                xmlEntry.SelectSingleNode("//ns:created", xmlnsmgr).InnerText = post.Modified.ToString("s");
                xmlEntry.SelectSingleNode("//ns:content", xmlnsmgr).InnerText = TransitPost.RenderXHTML(session, post);

                HttpWebRequest FeedRequest = (HttpWebRequest)WebRequest.Create(urlServicePost);
                if (!string.IsNullOrEmpty(mFeed.Username))
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
            throw new NotImplementedException();
        }
    }
}
