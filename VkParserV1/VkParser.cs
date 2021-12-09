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
        private readonly VkGroup _group;
        private readonly HttpClient _client;

        public VkParser(VkGroup @group, HttpClient client)
        {
            _group = @group;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private string BuildUrl()
        {
            return
                $"https://api.vk.com/method/wall.get?domain={HttpUtility.UrlEncode(_group.GroupName)}&count={_group.CountOfPosts}&access_token={HttpUtility.UrlEncode(_group.Token)}&v=5.81";
        }

        public async Task<string> GetJson()
        {
            var request = await _client.GetAsync(BuildUrl());
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
                    if (!string.IsNullOrEmpty(text))
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
                        var imageUrl = (string) listSizes[^1]["url"]!;
                        // listImages.Add($"[photo]({GetShortUrl(imageUrl).GetAwaiter().GetResult()})");
                        listImages.Add(imageUrl);
                    }
                    else
                    {
                        listImages.Add("");
                    }
                }
            }

            return listImages;
        }


        public List<VkVideo> GetVideo(string jsonString)
        {
            var listVideos = new List<VkVideo>();

            for (var i = 0; i < _group.CountOfPosts; i++)
            {
                var jToken = JObject.Parse(jsonString)["response"]?["items"]?[i]?["attachments"]?[0]?["video"];
                if (jToken != null)
                {
                    var info = GetVideoInfo(jToken).GetAwaiter().GetResult();
                    var previewUrl = info[0];
                    var videoUrl = info[1];
                    listVideos.Add(new VkVideo(previewUrl, videoUrl));
                }
                else
                {
                    jToken =
                        JObject.Parse(jsonString)["response"]?["items"]?[i]?["copy_history"]?[0]?["attachments"]?[0]?[
                            "video"];
                    if (jToken != null)
                    {
                        var info = GetVideoInfo(jToken).GetAwaiter().GetResult();
                        var previewUrl = info[0];
                        var videoUrl = info[1];
                        listVideos.Add(new VkVideo(previewUrl, videoUrl));
                    }
                    else
                    {
                        listVideos.Add(new VkVideo("", ""));
                    }
                }
            }
            
            return listVideos;
        }

        private async Task<string[]> GetVideoInfo(JToken? jToken)
        {
            var ownerId = (string) jToken["owner_id"]!;
            var videoId = (string) jToken["id"]!;
            var accessKey = (string) jToken["access_key"]!;
            var previewUrl = (string) jToken["photo_1280"]!;
            var requestVideoUrl =
                $"https://api.vk.com/method/video.get?videos=" +
                $"{ownerId}_" +
                $"{videoId}_" +
                $"{accessKey}" +
                $"&access_token={HttpUtility.UrlEncode(_group.Token)}&v=5.81";
            var request = await _client.GetAsync(requestVideoUrl);
            var jsonString = await request.Content.ReadAsStringAsync();
            jToken = JObject.Parse(jsonString)["response"]?["items"]?[0];
            var videoUrl = (string) jToken?["player"]!;

            string[] info = new string[]
            {
                previewUrl,
                videoUrl
            };

            return info;
        }

        private async Task<string> GetShortUrl(string url)
        {
            var client = new HttpClient();
            var requestUrl = "https://cutt.ly/scripts/shortenUrl.php";
            var request = await client.PostAsync(requestUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("url", url),
                new KeyValuePair<string?, string?>("domain", "0")
            }));
            return await request.Content.ReadAsStringAsync();
        }
    }
}