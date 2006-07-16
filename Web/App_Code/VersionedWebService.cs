using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Tools.Reflection;
using DBlog.Data.Hibernate;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace DBlog.WebServices
{
    public class VersionedWebService : DBlog.Data.Hibernate.WebService
    {
        #region System
        /// <summary>
        /// System version.
        /// </summary>
        [WebMethod(Description = "System version.", CacheDuration = 120)]
        public string GetVersion()
        {
            return AssemblyInfo<VersionedWebService>.Version;
        }

        /// <summary>
        /// System title.
        /// </summary>
        [WebMethod(Description = "System title.", CacheDuration = 120)]
        public string GetTitle()
        {
            return AssemblyInfo<VersionedWebService>.Title;
        }

        /// <summary>
        /// Product copyright.
        /// </summary>
        [WebMethod(Description = "Product copyright.", CacheDuration = 120)]
        public string GetCopyright()
        {
            return AssemblyInfo<VersionedWebService>.Copyright;
        }

        /// <summary>
        /// Product description.
        /// </summary>
        [WebMethod(Description = "Product description.", CacheDuration = 120)]
        public string GetDescription()
        {
            return AssemblyInfo<VersionedWebService>.Description;
        }

        /// <summary>
        /// Product uptime in ticks.
        /// </summary>
        [WebMethod(Description = "Product uptime in ticks.")]
        public long GetUptime()
        {
            return ((TimeSpan)(DateTime.UtcNow - Global.Started)).Ticks;
        }

        #endregion
    }
}
