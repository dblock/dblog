﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using DBlog.Tools.Web.Html;
using NUnit.Framework;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Trace;
using DBlog.Web.UnitTests.WebServices.Blog;

namespace DBlog.Web.UnitTests
{
    [TestFixture]
    public class Web
    {
        public const string sDBlogAuthCookieName = "DBlog.authcookie";
        public const string sDBlogPostCookieName = "DBlog.postcookie";

        [Test]
        protected void TestDeepGuestPerformance()
        {
            TestDeepPerformance(null);
        }

        public void TestPage(Uri root, Uri uri)
        {
            Cookie cookie = null;
            List<HtmlUri> links = null;
            double ts = 0;
            TestPage(root, uri, cookie, out links, out ts);
        }

        public void TestPage(Uri root, Uri uri, Cookie cookie, out List<HtmlUri> links, out double ts)
        {
            DateTime start = DateTime.UtcNow;
            Console.Write("{0} (from {1}) ...", uri.PathAndQuery, root.PathAndQuery);

            CookieContainer cookies = null;
            if (cookie != null)
            {
                cookies = new CookieContainer();
                cookies.Add(cookie);
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            if (cookies != null) request.CookieContainer = cookies;
            request.Method = "GET";
            request.AllowAutoRedirect = true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            string data = sr.ReadToEnd();

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, string.Format("Response code was {0}", response.StatusCode));
            Assert.IsFalse(string.IsNullOrEmpty(data));

            ts = DateTime.UtcNow.Subtract(start).TotalSeconds;
            Console.Write("[{0} bytes][{1} sec.]", data.Length, ts.ToString("0.00"));

            Assert.IsTrue(data.Length > 0, "Downloaded file size is zero.");

            if (response.ContentType != "text/html; charset=utf-8")
            {
                Console.WriteLine("[ignore: {0}]", response.ContentType);
                links = new List<HtmlUri>();
                return;
            }

            // login page
            if (response.ResponseUri.PathAndQuery.Contains("DBlog/Login.aspx") && response.ResponseUri.PathAndQuery.Contains("access=denied"))
            {
                Console.WriteLine("[ignore: {0}]", uri.PathAndQuery);
                links = new List<HtmlUri>();
                return;
            }

            int error_start = data.IndexOf("<div id=\"ctl00_noticeMenu_panelNotice\" class=\"notice_error\"");
            if (error_start > 0)
            {
                int error_end = data.IndexOf("</div>", error_start);
                string error = data.Substring(error_start, error_end - error_start);
                Assert.IsTrue(error_start < 0, string.Format("{0}: error: {1}", uri, error));
            }

            links = HtmlUriExtractor.Extract(data, root);
            // Console.WriteLine("\tExtracted {0} links.", links.Count);

            Console.WriteLine("[done]");
        }

        [Test]
        public void TestDeepGuest()
        {
            TestDeep(null);
        }

        [Test]
        public void TestDeepAdmin()
        {
            TestDeep("Administrator", "");
        }

        [Test]
        public void TestDeepUser()
        {
            TestDeep("User", "");
        }

        public void TestDeep(string username, string password)
        {
            Blog service = new Blog();
            string ticket = service.Login(username, password);
            Cookie cookie = new Cookie(sDBlogAuthCookieName, ticket, "/", "localhost");
            TestDeep(cookie);
        }

        public void TestDeepPerformance(Cookie cookie)
        {
            SqlConnectionInfo sci = SqlTrace.GetTraceSqlConnectionInfo();
            TestDeep(cookie, sci);
        }

        public void TestDeep(Cookie cookie)
        {
            TestDeep(cookie, null);
        }

        public void TestDeep(Cookie cookie, SqlConnectionInfo sci)
        {
            Uri root = new Uri("http://localhost/DBlog/Default.aspx");

            List<Uri> queue = new List<Uri>(2500);
            List<Uri> visited = new List<Uri>(5000);

            PerformanceDataCollection perfdata = (sci == null ? null : new PerformanceDataCollection());

            double totaltime = 0;
            int totalcount = 0;

            queue.Add(root);

            Dictionary<Uri, Uri> references = new Dictionary<Uri, Uri>();
            references.Add(root, new Uri("http://localhost/DBlog/"));

            while (queue.Count > 0)
            {
                Uri topofqueue = queue[0];
                List<HtmlUri> links;
                double ts = 0;

                PerformanceData perf = (perfdata != null ? new PerformanceData(topofqueue.ToString()) : null);
                try
                {
                    TraceServer traceServerReader = (sci == null ? null : SqlTrace.BeginTrace(sci));
                    TestPage(references[topofqueue], topofqueue, cookie, out links, out ts);
                    if (perfdata != null) perfdata.Add(SqlTrace.EndTrace(topofqueue.ToString(), traceServerReader));
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("{0}: {1}", topofqueue, ex.Message);
                    throw ex;
                }
                finally
                {
                    totalcount++;
                    totaltime += ts;
                    // add to visited links
                    visited.Add(topofqueue);
                    // remove from queue
                    queue.RemoveAt(0);
                }

                if (perfdata != null)
                {
                    perfdata.Add(perf);
                }

                // -----------------
                Console.Write("{0}/{1} [avg: {2}]", totalcount, queue.Count, (totaltime / totalcount).ToString("0.00"));
                if (perf != null) Console.Write("[SQL: {0} in {1}] ", perf.Queries, perf.TotalDuration);
                Console.Write(" => ");

                int count = 0;
                foreach (HtmlUri uri in links)
                {
                    Uri fulluri = uri.Uri;

                    if (!root.IsBaseOf(fulluri))
                    {
                        // Console.WriteLine("\tSkipping {0}.", uri);
                        continue;
                    }

                    if (references.ContainsKey(fulluri) || queue.Contains(fulluri) || visited.Contains(fulluri))
                        continue;

                    Assert.IsFalse(fulluri.ToString().Contains("\0"),
                        string.Format("Uri {0} in {1} contains non-ASCII character.", fulluri, topofqueue));

                    Assert.IsFalse(fulluri.ToString().Contains("<%"),
                        string.Format("Uri {0} in {1} contains non-executed ASP.NET code.", fulluri, topofqueue));

                    Assert.IsFalse(fulluri.ToString().Contains("id=0"),
                        string.Format("Uri {0} in {1} contains a link to a zero id.", fulluri, topofqueue));

                    references.Add(fulluri, topofqueue);

                    // Console.WriteLine("\tAdding {0}.", fulluri.OriginalString);
                    queue.Add(fulluri);
                    count++;
                }

                if ((perfdata != null) && (((totalcount > 0) && ((totalcount % 100) == 0)) || (queue.Count == 0)))
                {
                    perfdata.DumpPigs();
                }
            }
        }
    }
}
