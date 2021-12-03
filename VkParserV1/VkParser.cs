using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace NetCore.Docker
{
    public class VkParser
    {
        private readonly Database _db;
        private readonly string _filePath;
        private readonly VkGroup _group;

        public VkParser(VkGroup @group, Database db, string filePath)
        {
            _group = @group;
            _db = db;
            _filePath = filePath;
        }

        private string BuildUrl()
        {
            return
                $"https://api.vk.com/method/wall.get?domain={HttpUtility.UrlEncode(_group.GroupName)}&count={_group.CountOfPosts}&access_token={HttpUtility.UrlEncode(_group.Token)}&v=5.81";
        }

        public async Task<string> GetJson()
        {
            var client = new HttpClient();
            var request = await client.GetAsync(BuildUrl());
            var jsonString = await request.Content.ReadAsStringAsync();
            return jsonString;
        }

        public List<string> GetId(string jsonString)
        {
            var listId = new List<string>();
            for (var i = 0; i < _group.CountOfPosts; i++)
            {
                var jToken = JObject.Parse(jsonString)["response"]?["items"]?[i];
                if (jToken != null)
                {
                    listId.Add((string) jToken["id"]);
                }
            }

            return listId;
        }

        public List<string> GetText(string jsonString)
        {
            var listText = new List<string>();
            var str = "";
            for (var i = 0; i < _group.CountOfPosts; i++)
            {
                var jToken = JObject.Parse(jsonString)["response"]?["items"]?[i];
                if (jToken != null)
                {
                    var text = (string) jToken["text"];
                    if(!string.IsNullOrEmpty(text))
                    {
                        str = (string) jToken["text"];
                    }
                }

                jToken = JObject.Parse(jsonString)["response"]?["items"]?[i]?["copy_history"]?[0];
                if (jToken != null)
                {
                    str += "\n" + "" + "\n" + (string) jToken["text"];
                }

                listText.Add(HttpUtility.UrlEncode(str));
                str = "";
            }

            return listText;
        }

        public List<string> GetImage(string jsonString)
        {
            var listImages = new List<string>();
            for (var i = 0; i < _group.CountOfPosts; i++)
            {
                var jToken = JObject.Parse(jsonString)["response"]?["items"]?[i]?["attachments"]?[0]?["photo"];
                if (jToken != null)
                {
                    var listSizes = jToken["sizes"]!.ToList();
                    listImages.Add((string) listSizes[^1]["url"]);
                }
                else
                {
                    jToken =
                        JObject.Parse(jsonString)["response"]?["items"]?[i]?["copy_history"]?[0]?["attachments"]?[0]?[
                            "photo"];
                    if (jToken != null)
                    {
                        var listSizes = jToken["sizes"]!.ToList();
                        listImages.Add($"<a href={(string) listSizes[^1]["url"]!}>&#8205;</a>");
                    }
                    else
                    {
                        listImages.Add("");
                    }
                }
            }

            return listImages;
        }

        public List<Post> SaveToPost(List<string> listId, List<string> listText, List<string> listImages)
        {
            var listOfPosts = new List<Post>();
            if (listOfPosts == null) throw new ArgumentNullException(nameof(listOfPosts));
            if (listId == null || listText == null) return null;
            listOfPosts.AddRange(listId.Select((t, index) =>
                new Post(t, listText[index], listImages[index])));
            return listOfPosts;
        }

        public List<Post> SaveToPostWithFilter(List<string> listId, List<string> listText, List<string> listImages,
            string tag)
        {
            var listPosts = new List<Post>();
            if (listPosts == null) throw new ArgumentNullException(nameof(listPosts));
            listPosts.AddRange(listId.Select((t, i) => new Post(t, listText[i], listImages[i]))
                .Where((post, i) => listText[i].Contains(tag) && CompareIdInFile(post.Id)));
            return listPosts;
        }

        private bool CompareIdInList(Post post)
        {
            return _db.ListPosts.All(item => !item.Id.Equals(post.Id));
        }

        private bool CompareIdInFile(string id)
        {
            string[] fileStrings = File.ReadAllLines(_filePath);
            return !fileStrings.Contains(id);
        }
    }
}