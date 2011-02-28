using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DisqusScriptControl : DisqusControl
{
    private string mDisqusId; 
    private string mDisqusUrl;

    public string DisqusId
    {
        get
        {
            return mDisqusId;
        }
        set
        {
            mDisqusId = value;
        }
    }

    public string DisqusUrl
    {
        get
        {
            return mDisqusUrl;
        }
        set
        {
            mDisqusUrl = value;
        }
    }    
}