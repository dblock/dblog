using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace DBlog.Tools.WebControls
{
    public class WorkingButton : System.Web.UI.WebControls.Button
    {
        public WorkingButton()
        {
                        
        }

        protected override void OnLoad(EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (CausesValidation)
            {
                sb.Append("if (typeof(Page_ClientValidate) == 'function') { ");
                sb.Append("if (Page_ClientValidate() == false) { return false; }} ");
            }
            sb.Append("this.value = 'Working';");
            sb.Append("this.disabled = true;");
            sb.Append(Page.ClientScript.GetPostBackEventReference(this, string.Empty));
            sb.Append(";");
            this.Attributes.Add("onclick", sb.ToString()); 

            base.OnLoad(e);
        }
    }
}
