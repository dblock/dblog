using System;
using System.Reflection;
using System.Collections.Generic;

namespace DBlog.Tools.Reflection
{
    public class AssemblyInfo<T>
    {
        /// <summary>
        /// System version.
        /// </summary>
        public static string Version
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Major,
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Minor,
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Build,
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Revision);
            }
        }

        /// <summary>
        /// Product version.
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                return string.Format("{0}.{1}",
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Major,
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Minor);
            }
        }

        /// <summary>
        /// Build number.
        /// </summary>
        public static string ProductBuild
        {
            get
            {
                return string.Format("{0}.{1}",
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Build,
                    Assembly.GetAssembly(typeof(T)).GetName().Version.Revision);
            }
        }

        /// <summary>
        /// System title.
        /// </summary>
        public static string Title
        {
            get
            {
                return ((AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(
                    Assembly.GetAssembly(typeof(T)), typeof(AssemblyTitleAttribute))).Title;
            }
        }

        /// <summary>
        /// Product copyright.
        /// </summary>
        public static string Copyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
                    Assembly.GetAssembly(typeof(T)), typeof(AssemblyCopyrightAttribute))).Copyright;
            }
        }

        /// <summary>
        /// Product description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(
                    Assembly.GetAssembly(typeof(T)), typeof(AssemblyDescriptionAttribute))).Description;
            }
        }
    }
}
