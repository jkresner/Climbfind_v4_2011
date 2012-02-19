using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cf.Instrumentation
{
    /// <summary>
    /// Static methods to help us initialize trace listeners from web.config
    /// </summary>
    public static class TraceListenerConfigurationHelper
    {
        /// <summary>
        /// Parse a list of comma separated key value pairs to get a configuration value
        /// </summary>
        /// <param name="initializeData">The value contained in the initializeData attribute on the <add></add> element for the listener</param>
        /// <returns>Either a single file name or a fully qualified file path depending on configuration</returns>
        public static string GetFilePath(string initializeData)
        {
            string fileName = GetInitializeDataValue(initializeData, "FileName");

            if (String.IsNullOrEmpty(fileName))
            {
                throw new Exception("Cannot save log file because no 'FileName' value was supplied in initializeData attribute for Trace Listener");
            }

            string directory = GetInitializeDataValue(initializeData, "Directory");

            return (directory == null) ? fileName : Path.Combine(directory, fileName);
        }

        /// <summary>
        /// Parse a list of comma separated key value pairs to get a configuration value
        /// </summary>
        /// <param name="initializeData">The value contained in the initializeData attribute on the <add></add> element for the listener</param>
        /// <param name="key">The key we are searching for to get the corresponding value</param>
        /// <returns>Null if there is no matching key, else the value that corresponds to the key</returns>
        public static string GetInitializeDataValue(string initializeData, string key)
        {
            string result = null;

            if (!string.IsNullOrEmpty(initializeData))
            {
                List<string> configurationSettings = new List<string>(initializeData.Split(','));

                foreach (string configurationSetting in configurationSettings)
                {
                    string[] keyValue = configurationSetting.Trim().Split('=');

                    if (string.Equals(keyValue[0].Trim(), key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = keyValue[1].Trim();
                    }
                }
            }

            return result;
        }
    }
}
