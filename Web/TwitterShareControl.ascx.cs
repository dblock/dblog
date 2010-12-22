using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBlog.Tools.Web;

public partial class TwitterShareControl : TwitterControl
{
    private string mUrl;
    private string mText;

    public string Url
    {
        get
        {
            return mUrl;
        }
        set
        {
            mUrl = value;
        }
    }

    public string Text
    {
        get
        {
            return mText;
        }
        set
        {
            mText = value;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        if (!IsPostBack)
        {
            panelTwitterShare.Visible = TwitterEnabled;
            twitterLink.Attributes["data-url"] = Url;
            twitterLink.Attributes["data-text"] = Text;
            twitterLink.Attributes["data-count"] = "horizontal";
        }

        base.OnLoad(e);
    }
}