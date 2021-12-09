using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCore.Docker
{
    public class Application
    {
        private string _vkGroupName;
        private int _vkCountOfPosts;
        private string _vkToken;
        private string _tgToken;
        private string _tgChannelId;
        private string _tag;
        private string _databaseFilePath;
        private TelegramBot _telegramBot;
        private HttpClient _client;

        public Application(string vkGroupName, int vkCountOfPosts, string vkToken, string tgToken, string tgChannelId,
            string tag, string filePath, HttpClient client)
        {
            _vkGroupName = vkGroupName;
            _vkCountOfPosts = vkCountOfPosts;
            _vkToken = vkToken;
            _tgToken = tgToken;
            _tgChannelId = tgChannelId;
            _tag = tag;
            _databaseFilePath = filePath;
            _telegramBot = new TelegramBot(_tgToken, _tgChannelId);
            _client = client;

            if (!File.Exists(_databaseFilePath))
            {
                throw new FileNotFoundException(_databaseFilePath);
            }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: Newtonsoft.Json.Linq.JProperty")]
        public async Task Run()
        {
            var parser = new VkParser(new VkGroup(_vkGroupName, _vkCountOfPosts, _vkToken), _client);
            
            string vkResponceJson = parser.GetJson().GetAwaiter().GetResult();
            var listId = parser.GetId(vkResponceJson);
            var listText = parser.GetText(vkResponceJson);
            var listImages = parser.GetImage(vkResponceJson);
            var listVideos = parser.GetVideo(vkResponceJson);

            var postBuilder = new VkPostBuilder(_databaseFilePath);
            List<Post> savedPosts = postBuilder.SaveWithTag(listId, listText, listImages, listVideos, _tag);
            // PrintToConsole(savedPosts);

            foreach (Post post in savedPosts)
            {
                Console.WriteLine($"Posting id: '{post.Id}', text: '{post.Text.Replace("\n", "\\n").Substring(0, 64)} ...'");
                bool success = await _telegramBot.Post(post);
                if (success) {
                    File.AppendAllLines(_databaseFilePath, new[] {post.Id}); // add post.Id to fileDb
                }

                await Task.Delay(15 * 1000);
            }
        }

        private void PrintToConsole(List<Post> list)
        {
            foreach (var post in list)
            {
                Console.WriteLine("id = " + post.Id + "\n" + post.Text + "\n" + post.Image + "\n--- --- --- ---\n");
            }
        }
    }
}