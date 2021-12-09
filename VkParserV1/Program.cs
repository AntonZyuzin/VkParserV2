using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace NetCore.Docker
{
    class Program
    {
        static void Main(string[] args)
        {
            var vkGroupName = ConfigurationManager.AppSettings.Get("vkGroupName");
            var vkCountOfPosts = Convert.ToInt32(ConfigurationManager.AppSettings.Get("vkCountOfPosts"));
            var vkToken = ConfigurationManager.AppSettings.Get("vkToken");
            var tgToken = ConfigurationManager.AppSettings.Get("tgToken");
            var tgChannelId = ConfigurationManager.AppSettings.Get("tgChannelId");
            var tag = ConfigurationManager.AppSettings.Get("tag");
            var timer = Convert.ToInt32(ConfigurationManager.AppSettings.Get("timer"));
            var filePath = ConfigurationManager.AppSettings.Get("filePath");
            // var fileDatabasePath = "/Users/antonzyuzin/Documents/Projects/C#/VkParserV2_1/VkParserV1/posted.txt";
            string location = Assembly.GetExecutingAssembly().Location;
            string? directory = Path.GetDirectoryName(location);
            string fileDatabasePath = Path.GetFullPath(Path.Combine(directory, filePath));
            var client = new HttpClient();
            var app = new Application(vkGroupName, vkCountOfPosts, vkToken, tgToken, tgChannelId, tag, fileDatabasePath, client);
            
            while (true)
            {
                Console.WriteLine("начался обход");
                app.Run().GetAwaiter().GetResult();
                Console.WriteLine("сделан обход");
                System.Threading.Thread.Sleep(timer * 60 * 1000);
            }
        }
    }
}
