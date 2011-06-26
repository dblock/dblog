using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using System.Web.Hosting;
using System.Configuration;
using System.ComponentModel;

namespace DBlog.Tools.Web
{
    public class HostedApplication : System.Web.HttpApplication
    {
        private static DateTime mStarted;
        private EventLog mEventLog = null;

        static HostedApplication()
        {
            mStarted = DateTime.UtcNow;
        }

        public static DateTime Started
        {
            get
            {
                return mStarted;
            }
        }

        public HostedApplication()
        {
        }

        public static EventLog CreateEventLog()
        {
            string eventLogName = HostingEnvironment.ApplicationVirtualPath.Trim("/".ToCharArray());
            if (eventLogName.Length == 0) eventLogName = HostingEnvironment.SiteName;
            if (eventLogName.Length == 0) eventLogName = "Application";

            if (!EventLog.SourceExists(eventLogName))
            {
                EventLog.CreateEventSource(eventLogName, "Application");
            }

            EventLog result = new EventLog();
            result.Source = eventLogName;
            return result;
        }

        public EventLog EventLog
        {
            get
            {
                if (mEventLog == null && EventLogEnabled)
                {
                    mEventLog = CreateEventLog();
                }
                return mEventLog;
            }
        }

        protected virtual void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();

            if (EventLogEnabled)
            {
                EventLog.WriteEntry(String.Format("Application error in {0}.\n{1}\n\n{2}",
                    Request.Url.ToString(),
                    ex.Message,
                    ex.StackTrace),
                    EventLogEntryType.Error);
            }
        }

        protected virtual void Application_Start(Object sender, EventArgs e)
        {
            if (EventLogEnabled)
            {
                EventLog.WriteEntry("Application starting.");
            }
        }

        protected virtual void Application_End(Object sender, EventArgs e)
        {
            HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember(
                "_theRuntime",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField,
                null, null, null);

            if (runtime == null)
                return;

            string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

            string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

            if (EventLogEnabled)
            {
                EventLog.WriteEntry(String.Format("{0}\n{1}\n\n{2}",
                    System.Web.Hosting.HostingEnvironment.ShutdownReason,
                    shutDownMessage,
                    shutDownStack),
                    EventLogEntryType.Warning);
            }
        }

        public static bool EventLogEnabled
        {
            get
            {
                return IsEnabled("EventLog");
            }
        }

        public static bool ServicesEnabled
        {
            get
            {
                return IsEnabled("Services");
            }
        }

        public static bool IsEnabled(string name)
        {
            bool b_result = true;

            object result = ConfigurationManager.AppSettings[string.Format("{0}.Enabled", name)];

            if (result != null)
            {
                bool.TryParse(result.ToString(), out b_result);
            }

            return b_result;
        }
    }
}
