using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NetCore.Docker
{
    public class VkParser
    {
        private readonly Database _db;
        private readonly VkGroup _group;

        public VkParser(VkGroup @group, Database db)
        {
            _group = @group;
            _db = db;
        }

        private string BuildUrl()
        {
            return
                $"https://api.vk.com/method/wall.get?domain={_group.GroupName}&count={_group.CountOfPosts}&access_token={_group.Token}&v=5.81";
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

                listText.Add(str);
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
                        listImages.Add((string) listSizes[^1]["url"]);
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
                .Where((post, i) => listText[i].Contains(tag) && CompareId(post)));

            return listPosts;
        }

        private bool CompareId(Post post)
        {
            return _db.ListPosts.All(item => !item.Id.Equals(post.Id));
        }
    }
}