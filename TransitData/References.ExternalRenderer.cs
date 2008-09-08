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

namespace DBlog.TransitData.References
{
    public abstract class ExternalRenderer
    {
        protected ISession mSession = null;

        public ExternalRenderer(ISession session)
        {
            mSession = session;
        }

        public abstract string Render(string value);

        protected string ReferUrl(string url, string content)
        {
            return string.Format("<a target='_blank' href='{0}'>{1}</a>",
                url, content);
        }
    }
}