using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using DBlog.TransitData;
using System.Collections.Generic;
using System.Web.Caching;
using DBlog.Tools.Web;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data;

namespace DBlog.TransitData.References
{
    public class ReferencesRedirector
    {
        protected int mId = 0;
        protected string mType = null;

        public ReferencesRedirector(int id, string type)
        {
            mId = id;
            mType = type;
        }

        public string ReferUri
        {
            get
            {
                string rooturi = ConfigurationManager.AppSettings["url"];
                return string.Format("{0}ShowUrl.aspx?ObjectId={1}&amp;ObjectType={2}",
                    rooturi, mId, mType);
            }
        }

        public string Refer(string uri)
        {
            string rooturi = ConfigurationManager.AppSettings["url"];
            return string.Format("{0}&amp;Url={1}", ReferUri, Renderer.UrlEncode(uri));
        }
    }
}