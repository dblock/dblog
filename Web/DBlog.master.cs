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

public partial class DBlogMaster : MasterPage
{
    public event TopicsViewControl.TopicChangedHandler TopicChanged;

    protected void Page_Load(object sender, EventArgs e)
    {
        topics.TopicChanged += new TopicsViewControl.TopicChangedHandler(topics_TopicChanged);        
    }

    public void topics_TopicChanged(object sender, TopicsViewControl.TopicChangedEventArgs e)
    {
        if (TopicChanged != null)
        {
            TopicChanged(sender, e);
        }
    }
}
