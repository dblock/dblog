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
using System.IO;
using System.Text;

public partial class EditFeed : BlogAdminPage
{
    private TransitFeed mFeed = null;

    public TransitFeed Feed
    {
        get
        {
            if (mFeed == null)
            {
                mFeed = (RequestId > 0)
                    ? SessionManager.BlogService.GetFeedById(SessionManager.Ticket, RequestId)
                    : new TransitFeed();
            }

            return mFeed;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetDefaultButton(save);

                ListItemCollection intervals = new ListItemCollection();
                intervals.Add(new ListItem("Never", Convert.ToString(-1)));
                intervals.Add(new ListItem("Every Request", Convert.ToString(0)));
                intervals.Add(new ListItem("One Minute", Convert.ToString(60)));
                intervals.Add(new ListItem("Five Minutes", Convert.ToString(5 * 60)));
                intervals.Add(new ListItem("Ten Minutes", Convert.ToString(10 * 60)));
                intervals.Add(new ListItem("Half Hour", Convert.ToString(30 * 60)));
                intervals.Add(new ListItem("One Hour", Convert.ToString(60 * 60)));
                intervals.Add(new ListItem("Twelve Hours", Convert.ToString(12 * 60 * 60)));
                intervals.Add(new ListItem("One Day", Convert.ToString(24 * 60 * 60)));

                inputInterval.DataSource = intervals;
                inputInterval.DataBind();

                inputType.DataSource = Enum.GetValues(typeof(TransitFeedType));
                inputType.DataBind();

                if (RequestId > 0)
                {
                    inputName.Text = Feed.Name;
                    inputUrl.Text = Feed.Url;
                    inputDescription.Text = Feed.Description;
                    inputXsl.PostedFile = new UploadControl.HttpPostedFile(string.IsNullOrEmpty(Feed.Xsl) ? "None" : string.Format("{0} bytes", Feed.Xsl.Length));

                    ListItem li = inputInterval.Items.FindByValue(Feed.Interval.ToString());
                    if (li == null)
                    {
                        li = new ListItem(string.Format("{0} Seconds", Feed.Interval), Feed.Interval.ToString());
                        inputInterval.Items.Add(li);
                    }

                    inputInterval.ClearSelection();
                    li.Selected = true;

                    inputUsername.Text = Feed.Username;
                    inputPassword.Attributes["value"] = Feed.Password;

                    inputType.Items.FindByValue(Feed.Type.ToString()).Selected = true;
                }
            }
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }

    public void save_Click(object sender, EventArgs e)
    {
        try
        {
            inputPassword.Attributes["value"] = inputPassword.Text;

            if (inputXsl.HasNewData)
            {
                Feed.Xsl = Encoding.Default.GetString(inputXsl.PostedFile.Data);
            }

            Feed.Name = CheckInput("Name", inputName.Text);
            Feed.Url = CheckInput("Url", inputUrl.Text);
            Feed.Description = inputDescription.Text;
            Feed.Type = (TransitFeedType) Enum.Parse(typeof(TransitFeedType), inputType.SelectedValue);

            SessionManager.BlogService.CreateOrUpdateFeed(SessionManager.Ticket, Feed);
            Response.Redirect("ManageFeeds.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
