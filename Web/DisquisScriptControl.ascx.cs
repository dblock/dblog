using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DisquisScriptControl : DisquisControl
{
    private string mDisquisId;
    private string mDisquisUrl;

    public string DisquisId
    {
        get
        {
            return mDisquisId;
        }
        set
        {
            mDisquisId = value;
        }
    }

    public string DisquisUrl
    {
        get
        {
            return mDisquisUrl;
        }
        set
        {
            mDisquisUrl = value;
        }
    }    
}