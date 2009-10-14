using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using DBlog.Tools.Drawing;
using System.Xml;
using System.Text;
using System.Threading;

namespace DBlog.Tools.Web
{
    public abstract class XmlPage : Page
    {
        public abstract DateTime LastModified { get; }
        public abstract string XmlContentType { get; }
        public abstract void WriteXml(XmlWriter w);

        public Nullable<DateTime> IfModifiedSince
        {
            get
            {
                Nullable<DateTime> result = new Nullable<DateTime>();
                object o = Request.Headers["If-Modified-Since"];
                if (o == null) return result;
                string s = o.ToString().Split(';')[0];
                DateTime dt;
                if (DateTime.TryParse(s, out dt)) result = dt;
                return result;
            }
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (XmlTextWriter writer = new XmlTextWriter(Response.OutputStream, Encoding.UTF8))
                {
                    Nullable<DateTime> ims = IfModifiedSince;

                    if (ims.HasValue)
                    {
                        if (ims.Value.ToUniversalTime() >= LastModified)
                        {
                            Response.StatusCode = 304;
                            return;
                        }
                    }

                    Response.Clear();
                    Response.Cache.SetLastModified(LastModified.ToLocalTime());
                    Response.Cache.SetCacheability(HttpCacheability.Private);
                    Response.ContentType = XmlContentType;
                    Response.AddHeader("Modified", LastModified.ToString("r"));
                    WriteXml(writer);
                    writer.Close();
                }

                Response.End();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}