using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBlog.TransitData;

public partial class ShowZenGalleryFull : BlogPage
{
    private TransitTopic mTopic = null;

    public TransitTopic Topic
    {
        get
        {
            if (mTopic == null)
            {
                mTopic = SessionManager.GetCachedObject<TransitTopic>("GetTopicById", SessionManager.Ticket,
                    RequestId);
            }

            return mTopic;
        }
    }

    public string GalleryXml
    {
        get
        {
            return string.Format("{0}/galleries/{1}.xml",
                SessionManager.WebsiteUrl,
                Topic.Name);
        }
    }

    public void Page_Load(object sender, EventArgs args)
    {
    }

    public void linkContact_Click(object sender, EventArgs e)
    {
        Page.ClientScript.RegisterStartupScript(this.GetType(), "email",
            string.Format("<script language='JavaScript'>window.location.href='mailto:{0}';</script>",
            SessionManager.GetSetting("email", "admin@localhost.com")));
    }
}
