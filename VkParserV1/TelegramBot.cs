using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NetCore.Docker
{
    public class TelegramBot
    {
        private string _token;
        private string _chanelId;
        private HttpClient _client;

        public TelegramBot(string token, string chanelId)
        {
            _token = token;
            _chanelId = chanelId;
            _client = new HttpClient();
        }

        private string BuildUrl(Post post)
        {
            string text = EscapeTelegramReservedCharacters(post.Text);
            string imageUrl = EscapeTelegramReservedCharacters(post.Image);
            string previewUrl = EscapeTelegramReservedCharacters(post.Video.PreviewUrl);
            string videoUrl = EscapeTelegramReservedCharacters(post.Video.VideoUrl);
            string textOfPost = text;
            
            if (!imageUrl.Equals(""))
            {
               textOfPost += "\n\n" + $"[photo]({imageUrl})";
            }
            else if (!previewUrl.Equals(""))
            {
                textOfPost += "\n\n" + $"[preview]({previewUrl})";
            }
            
            // if (!videoUrl.Equals(""))
            // {
            //     textOfPost += "\n" + $"[video]({videoUrl})";
            // }



            string url = "https://api.telegram.org"
                         + $"/bot{_token}"
                         + $"/sendMessage"
                         + $"?chat_id={_chanelId}"
                         + $"&parse_mode=MarkdownV2"
                         + $"&text={HttpUtility.UrlEncode(textOfPost)}";

            return url;
        }


        // private async Task<string> GetShortUrl(string url)
        // {
        //     var client = new HttpClient();
        //     string requestUrl = "https://cutt.ly/scripts/shortenUrl.php";
        //     var request = await client.PostAsync(requestUrl, new FormUrlEncodedContent(new []
        //     {
        //         new KeyValuePair<string?, string?>("url", url),
        //         new KeyValuePair<string?, string?>("domain", "0")
        //     }));
        //     return await request.Content.ReadAsStringAsync();
        // }
        
        public async Task<bool> Post(Post post)
        {
            // string url = BuildUrl(HttpUtility.UrlEncode(text));
            string url = BuildUrl(post);
            Console.WriteLine($"Telegram Get url: '{url}'.");
            HttpResponseMessage response = await _client.GetAsync(url);
            Console.WriteLine("пост сделан");
            Console.WriteLine($"Response: code: '{response.StatusCode}', reason: '{response.ReasonPhrase}', content: '{await response.Content.ReadAsStringAsync()}'");
            return response.IsSuccessStatusCode;
        }

        private string EscapeTelegramReservedCharacters(string text)
        {
            string[] telegramReservedCharacters = new[] { "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
            foreach (string reservedCharacter in telegramReservedCharacters)
            {
                text = text.Replace(reservedCharacter, @$"\{reservedCharacter}");
            }
            return text;
        }
    }
}