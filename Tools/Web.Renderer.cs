using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace DBlog.Tools.Web
{
    public class Renderer
    {
        public Renderer()
        {

        }

        public static string Render(object message)
        {
            if (message == null) return string.Empty;
            return Render(message.ToString());
        }

        public static string Render(string message)
        {
            if (message == null) return string.Empty;
            string result = HttpUtility.HtmlEncode(message);
            if (result.IndexOf("<P>", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                result = result.Replace("\n", "<br/>\n");
            }
            return result;
        }

        public static string RenderEx(object message, string basehref, string rewritehref)
        {
            if (message == null) return string.Empty;
            return RenderEx(message.ToString(), basehref, rewritehref);
        }

        public static string RenderEx(string message, string basehref, string rewritehref)
        {
            string result = CleanHtml(message, basehref, rewritehref);
            if (result.IndexOf("<P>", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                result = result.Replace("\n", "<br/>\n");
            }
            return result;
        }

        public static Regex HtmlExpression = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
        public static Regex BrExpression = new Regex(@"<br[\/ ]*>", RegexOptions.IgnoreCase);

        public static string RemoveHtml(object message)
        {
            if (message == null) return string.Empty;
            return RemoveHtml(message.ToString());
        }

        public static string RemoveHtml(string message)
        {
            message = BrExpression.Replace(message, "\n");
            message = HtmlExpression.Replace(message, string.Empty);
            message = message.Trim('\n');
            return message;
        }

        public static string CleanHtml(object message)
        {
            if (message == null) return string.Empty;
            return CleanHtml(message.ToString());
        }

        public static string CleanHtml(string html)
        {
            return CleanHtml(html, null, null);
        }

        public static string CleanHtml(string html, string basehref, string rewritehref)
        {
            try
            {
                Html.HtmlReader r = new Html.HtmlReader(html);
                StringWriter sw = new StringWriter();
                Html.HtmlWriter w = new Html.HtmlWriter(sw);
                if (!string.IsNullOrEmpty(basehref)) w.Options.BaseHref = new Uri(basehref);
                if (!string.IsNullOrEmpty(rewritehref)) w.Options.RewriteHref = new Uri(rewritehref);
                while (! r.EOF)
                {
                    w.WriteNode(r, true);
                }
                w.Close();
                return sw.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string UrlEncode(string message)
        {
            if (message == null) return string.Empty;
            return HttpUtility.UrlEncode(message);
        }

        public static string UrlEncode(object message)
        {
            if (message == null) return string.Empty;
            return UrlEncode(message.ToString());
        }

        public static string UrlDecode(string message)
        {
            if (message == null) return string.Empty;
            return HttpUtility.UrlDecode(message);
        }

        public static string UrlDecode(object message)
        {
            if (message == null) return string.Empty;
            return UrlDecode(message.ToString());
        }

        static string HrefHandler(Match ParameterMatch)
        {
            string result = ParameterMatch.Value;

            // quoted results within other tags are ignored
            if (result.StartsWith("\"") && result.EndsWith("\""))
                return result;

            string afterresult = string.Empty;
            const string punctutation = ".,;:";
            while ((result.Length > 0) && (punctutation.IndexOf(result[result.Length - 1]) >= 0))
            {
                afterresult = afterresult + result[result.Length - 1];
                result = result.Remove(result.Length - 1, 1);
            }

            string shortenedresult = result;
            
            int cut = shortenedresult.IndexOf("?");
            if (cut >= 0)
            {
                shortenedresult = shortenedresult.Substring(0, cut) + " ...";
            }

            result = RemoveMarkups(result);

            result = string.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>{2}",
                result,
                shortenedresult,
                afterresult);

            return result;
        }

        public static Regex HrefExpression = new Regex(@"[\'\""]{0,1}[\w]+://[a-zA-Z0-9\/\,\(\)\.\?\&+\##\%~=:;_\-\@]*[\'\""]{0,1}", RegexOptions.IgnoreCase);

        public static string RenderHref(string RenderValue)
        {
            MatchEvaluator HrefHandlerDelegate = new MatchEvaluator(HrefHandler);
            return HrefExpression.Replace(RenderValue, HrefHandlerDelegate);
        }

        class NameValueMap : StringDictionary 
        {
            public NameValueMap()
            {
                Add("[small]", "<small>");
                Add("[/small]", "</small>");
                Add("[h1]", "<h1>");
                Add("[h2]", "<h2>");
                Add("[h3]", "<h3>");
                Add("[big]", "<big>");
                Add("[/h1]", "</h1>");
                Add("[/h2]", "</h2>");
                Add("[/h3]", "</h3>");
                Add("[/big]", "</big>");
                Add("[b]", "<b>");
                Add("[/b]", "</b>");
                Add("[em]", "<em>");
                Add("[/em]", "</em>");
                Add("[i]", "<em>");
                Add("[/i]", "</em>");
                Add("[red]", "<font color=\"red\">");
                Add("[/red]", "</font>");
                Add("[green]", "<font color=\"green\">");
                Add("[/green]", "</font>");
                Add("[blue]", "<font color=\"blue\">");
                Add("[/blue]", "</font>");
                Add("[img]", "<img border=\"0\" src=\"");
                Add("[/img]", "\">");
                Add("[image]", "<img border=\"0\" src=\"");
                Add("[/image]", "\"/>");
                Add("[center]", "<div style=\"text-align: center;\">");
                Add("[/center]", "</div>");
                Add("[code]", "<div class=code><pre>");
                Add("[/code]", "</pre></div>");
            }
        };

        static NameValueMap sMarkupMap = new NameValueMap();

        static string MarkupHandler(Match ParameterMatch)
        {
            string word = ParameterMatch.Value;

            if (word.StartsWith("[[") && word.EndsWith("]]"))
                return word.Substring(1, word.Length - 2);

            string result = sMarkupMap[word];
            return string.IsNullOrEmpty(result) ? word : result;
        }

        static string MarkupClearHandler(Match ParameterMatch)
        {
            return string.Empty;
        }

        static Regex MarkupExpression = new Regex(@"[\[]+\/*\w*[\]]+", RegexOptions.IgnoreCase);

        public static string RenderMarkups(string RenderValue)
        {
            MatchEvaluator MarkupHandlerDelegate = new MatchEvaluator(MarkupHandler);
            return MarkupExpression.Replace(RenderValue, MarkupHandlerDelegate);
        }

        public static string RemoveMarkups(string RenderValue)
        {
            MatchEvaluator MarkupHandlerDelegate = new MatchEvaluator(MarkupClearHandler);
            return MarkupExpression.Replace(RenderValue, MarkupHandlerDelegate);
        }

        public static string SqlEncode(string Value)
        {
            return Value.Replace("'", "''");
        }

        public static string GetSummary(string summary)
        {
            string result = RemoveHtml(RemoveMarkups(summary));
            if (result.Length > 256) result = result.Substring(0, 256) + " ...";
            return result;
        }

        public static string GetLink(string uri, string text)
        {
            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", uri, text);
        }

        public static string ToRfc822(DateTime value)
        {
            return string.Format("{0} GMT",
                value.ToString("ddd, dd MMM yyyy HH:mm:ss"));
        }

        /// <summary>
        /// Transform a string into a slug.
        /// See http://www.intrepidstudios.com/blog/2009/2/10/function-to-generate-a-url-friendly-string.aspx
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSlug(string s)
        {
            s = s.ToLower();
            // invalid chars, make into spaces
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
            // convert multiple spaces/hyphens into one space       
            s = Regex.Replace(s, @"[\s-]+", " ").Trim();
            // hyphens
            s = Regex.Replace(s, @"\s", "-");
            return s;
        }
    }
}