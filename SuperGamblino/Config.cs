using Newtonsoft.Json.Linq;
using SuperGamblino.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace SuperGamblino
{
    internal static class Config
    {
        internal static string token = "";
        internal static string prefix = "";
        internal static string logLevel = "Info";
        internal static string colorInfo = "";
        internal static string colorSuccess = "";
        internal static string colorWarning = "";

        public static void LoadConfig()
        {
            // Writes default config to file if it does not already exist
            //if (!File.Exists("./config.yml"))
            //{
            //    File.WriteAllText("./config.yml", Encoding.UTF8.GetString(Resources.DefaultConfig));
            //}

            if (!File.Exists("./config.json"))
            {
                File.WriteAllText("./config.json", Encoding.UTF8.GetString(Resources.DefaultConfig));
            }

            string test = File.ReadAllText("./config.json");
            JObject json = JObject.Parse(test);

            //Bot setup
            token = json.SelectToken("bot.token").Value<string>();
            prefix = json.SelectToken("bot.prefix").Value<string>();
            //Colors
            colorInfo = json.SelectToken("color.info").Value<string>();
            colorSuccess = json.SelectToken("color.success").Value<string>();
            colorWarning = json.SelectToken("color.warning").Value<string>();

        }
    }
}
