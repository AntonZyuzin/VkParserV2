using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Configuration;

namespace NetCore.Docker
{
    public class Application
    {
        private Database _db;
        private string _vkGroupName;
        private int _vkCountOfPosts;
        private string _vkToken;
        private string _tgToken;
        private string _tgChannelId;
        private string _tag;
        public Application(Database db, string vkGroupName, int vkCountOfPosts, string vkToken, string tgToken, string tgChannelId, string tag)
        {
            _db = db;
            _vkGroupName = vkGroupName;
            _vkCountOfPosts = vkCountOfPosts;
            _vkToken = vkToken;
            _tgToken = tgToken;
            _tgChannelId = tgChannelId;
            _tag = tag;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: Newtonsoft.Json.Linq.JProperty")]
        public void Start()
        {
            var group = new VkGroup(_vkGroupName, _vkCountOfPosts, _vkToken);
            var parser = new VkParser(group, _db);
            var bot = new TelegramBot(_tgToken, _tgChannelId);
            var jsonString = parser.GetJson().GetAwaiter().GetResult();
            var listId = parser.GetId(jsonString);
            var listText = parser.GetText(jsonString);
            var listImages = parser.GetImage(jsonString);
            var listPostsSorted = parser.SaveToPostWithFilter(listId, listText, listImages, _tag);
            _db.ListPosts.AddRange(listPostsSorted);
            var list = PostToString(listPostsSorted);
            // var list = PostToString(listPosts);
            foreach (var item in list)
            {
                bot.DoPost(item).GetAwaiter().GetResult();
                System.Threading.Thread.Sleep(15 * 1000);
            }
        }

        private void PrintListToConsole(List<Post> list)
        {
            foreach (var post in list)
            {
                Console.WriteLine("id = " + post.Id + "\n" + post.Text + "\n" + post.Image + "\n--- --- --- ---\n");
            }
        }

        private List<string> PostToString(List<Post> list)
        {
            var listStrPosts = list.Select(post => post.Text + "\n\n" + post.Image).ToList();
            return listStrPosts;
        }
    }
}