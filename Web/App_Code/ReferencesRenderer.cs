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
using DBlog.WebServices;
using DBlog.Tools.Web;

public class ReferencesRenderer : ExternalRenderer
{
    public ReferencesRenderer(Page page, int id, string type)
        : base(page, id, type)
    {

    }

    private string RefHandler(Match ParameterMatch)
    {
        string word = ParameterMatch.Value;

        TransitReference reference = (TransitReference) mPage.Cache[string.Format("ref:{0}", word)];

        if (reference == null)
        {
            reference = mPage.SessionManager.BlogService.GetReferenceByWord(mPage.SessionManager.Ticket, word);

            if (reference != null)
            {
                mPage.Cache.Insert(string.Format("ref:{0}", word), reference);
            }
        }

        if (reference == null)
        {
            return string.Format("[error: {0} not found]", word);
        }

        string image = "link";

        if (reference.Url.IndexOf("citysearch.com/") > 0)
            image = "citysearch";

        string content = string.Format("<img src='{0}images/links/{1}.gif' border='0' align='absmiddle' width='16' height='16' /> {2}",
            mPage.SessionManager.GetSetting("url", string.Empty),
            image,
            reference.Result);

        return ReferUrl(reference.Url, content);
    }

    private static Regex RefExpression = new Regex(@"\w+\:\w+", RegexOptions.IgnoreCase);

    public override string Render(string value)
    {
        MatchEvaluator RefHandlerDelegate = new MatchEvaluator(RefHandler);
        return RefExpression.Replace(value, RefHandlerDelegate);
    }
}
