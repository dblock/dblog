using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace DBlog.Tools.Web
{
    public class ViewState<T>
    {
        public static T GetViewStateValue(StateBag bag, string key, T defaultvalue)
        {
            object result = bag[key];
            if (result == null) return defaultvalue;
            return (T) result;
        }

        public static T GetViewStateValue(bool EnableViewState, StateBag bag, string key, T defaultvalue)
        {
            if (EnableViewState)
            {
                return GetViewStateValue(bag, key, defaultvalue);
            }
            else
            {
                return defaultvalue;
            }
        }

        public static void SetViewStateValue(bool EnableViewState, StateBag bag, string key, T value, ref T member)
        {
            if (EnableViewState)
            {
                bag[key] = value;
            }

            member = value;
        }
    }
}
