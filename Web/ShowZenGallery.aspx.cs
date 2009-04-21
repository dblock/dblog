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

public partial class ShowZenGallery : BlogPage
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
