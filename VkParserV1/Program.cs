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
            db.ListPosts = new List<Post>();
            var app = new Application(db, vkGroupName, vkCountOfPosts, vkToken, tgToken, tgChannelId, tag);
            StartApp(db, app, timer);
        }

        private static void StartApp(Database db, Application app, int timer)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(timer * 60 * 1000);
                app.Start();
            });
            t.Wait();
            Console.WriteLine("сделан обход");
            StartApp(db, app, timer);
            db.CleanList();
        }
    }
}