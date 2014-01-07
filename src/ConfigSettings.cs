using System.Configuration;
using ServiceStack;
using ServiceStack.Common;

namespace iLinksEditor
{
    public class ConfigSettings
    {
        private static readonly ConfigSettings instance = new ConfigSettings();

        
        public static ConfigSettings Current { get { return instance; } }
        private ConfigSettings()
        { }

        public string JetNettApiAddress
        {
            get
            {
                //#if DEBUG
                //    return "http://localhost:9037/";
                //#endif
                var s = ConfigurationManager.AppSettings["jetnett-api"];
                return !s.IsNullOrEmpty() ? s : "";
            } 
        }
    }
}