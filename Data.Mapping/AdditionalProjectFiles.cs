using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DBlog.Data.Mapping
{
    public enum AdditionalProjectFileConfigurationSubType
    {
        File,
        Link
    }

    public class AdditionalProjectFileConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("filename", IsRequired = true)]
        public String Filename
        {
            get
            {
                return (String)this["filename"];
            }
            set
            {
                this["filename"] = value;
            }
        }

        [ConfigurationProperty("subtype", IsRequired = false)]
        public AdditionalProjectFileConfigurationSubType SubType
        {
            get
            {
                return (AdditionalProjectFileConfigurationSubType)this["subtype"];
            }
            set
            {
                this["subtype"] = value;
            }
        }
    }

    public class AdditionalProjectFileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AdditionalProjectFileConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AdditionalProjectFileConfigurationElement) element).Filename;
        }
    }

    public class AdditionalProjectFilesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true, IsRequired = true)]
        public AdditionalProjectFileCollection AdditionalProjectFiles
        {
            get
            {
                return (AdditionalProjectFileCollection) this[""];
            }
        }
    }
}
