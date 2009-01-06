using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBlog.Tools.Web.Html
{
    public class Cutter
    {
        public static string Cut(string value, int id)
        {
            int cutBeginPos = value.IndexOf("[cut]");
            if (cutBeginPos >= 0)
            {
                int cutEndPos = value.IndexOf("[/cut]", cutBeginPos);
                if (cutEndPos > cutBeginPos)
                {
                    value = value
                        .Remove(cutEndPos, value.Length - cutEndPos)
                        .Insert(cutEndPos, "</a>")
                        .Remove(cutBeginPos, "[cut]".Length)
                        .Insert(cutBeginPos, string.Format("<a href=\"ShowPost.aspx?id={0}\">", id));
                }
            }
            return value;
        }

        public static string DeleteCut(string value)
        {
            int cutBeginPos = value.IndexOf("[cut]");
            if (cutBeginPos >= 0)
            {
                int cutEndPos = value.IndexOf("[/cut]", cutBeginPos);
                if (cutEndPos > cutBeginPos)
                {
                    return value.Remove(cutBeginPos, 
                        cutEndPos - cutBeginPos + "[/cut]".Length);
                }
            }
            return value;
        }
    }
}
