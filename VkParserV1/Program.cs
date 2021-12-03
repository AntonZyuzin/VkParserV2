using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace NetCore.Docker
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database();
            var vkGroupName = ConfigurationManager.AppSettings.Get("vkGroupName");
            var vkCountOfPosts = Convert.ToInt32(ConfigurationManager.AppSettings.Get("vkCountOfPosts"));
            var vkToken = ConfigurationManager.AppSettings.Get("vkToken");
            var tgToken = ConfigurationManager.AppSettings.Get("tgToken");
            var tgChannelId = ConfigurationManager.AppSettings.Get("tgChannelId");
            var tag = ConfigurationManager.AppSettings.Get("tag");
            var timer = Convert.ToInt32(ConfigurationManager.AppSettings.Get("timer"));
            var filePath = ConfigurationManager.AppSettings.Get("filePath");
            var app = new Application(db, vkGroupName, vkCountOfPosts, vkToken, tgToken, tgChannelId, tag, filePath);
            while (true)
            {
                Console.WriteLine("начался обход");
                app.Start();
                Console.WriteLine("сделан обход");
                db.CleanList();
                System.Threading.Thread.Sleep(timer * 60 * 1000);
            }
        }
    }
}
