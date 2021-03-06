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

public partial class EditReference : BlogAdminPage
{
    private TransitReference mReference = null;

    public TransitReference Reference
    {
        get
        {
            if (mReference == null)
            {
                mReference = (RequestId > 0)
                    ? SessionManager.GetCachedObject<TransitReference>("GetReferenceById", SessionManager.Ticket, RequestId)
                    : new TransitReference();
            }

            return mReference;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                SetDefaultButton(save);

                if (RequestId > 0)
                {
                    inputWord.Text = Reference.Word;
                    inputUrl.Text = Reference.Url;
                    inputResult.Text = Reference.Result;
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
            Reference.Word = CheckInput("Word", inputWord.Text);
            Reference.Url = CheckInput("Url", inputUrl.Text);
            Reference.Result = CheckInput("Result", inputResult.Text);
            SessionManager.BlogService.CreateOrUpdateReference(SessionManager.Ticket, Reference);
            SessionManager.Invalidate<TransitReference>();
            Response.Redirect("./ManageReferences.aspx");
        }
        catch (Exception ex)
        {
            ReportException(ex);
        }
    }
}
