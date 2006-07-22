using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DBlog.Tools.Web
{
    public class Page : System.Web.UI.Page
    {
        public Page()
        {

        }

        public int RequestId
        {
            get
            {
                return GetId(DefaultId);
            }
        }

        public int GetId(string querystring)
        {
            string id = Request.QueryString[querystring];
            if (string.IsNullOrEmpty(id)) return 0;
            return int.Parse(id);
        }

        public virtual string DefaultId
        {
            get
            {
                return "id";
            }
        }

        public void SetDefaultButton(Button button)
        {
            PageManager.SetDefaultButton(button, this.Controls);
        }
    }
}
