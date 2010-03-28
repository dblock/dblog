using System;

public partial class AtomBlog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Redirect("AtomPost.aspx");
        Response.End();
    }
}