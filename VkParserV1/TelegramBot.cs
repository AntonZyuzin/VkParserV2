using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NetCore.Docker
{
    public class TelegramBot
    {
        public TelegramBot(string token, string chanelId)
        {
            _token = token;
            _chanelId = chanelId;
        }

        private string _token;
        private string _chanelId;

        private string BuildUrl(string text)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://api.telegram.org/bot");
            stringBuilder.Append(HttpUtility.UrlEncode(_token));
            stringBuilder.Append("/sendMessage?chat_id=");
            stringBuilder.Append(HttpUtility.UrlEncode(_chanelId));
            stringBuilder.Append("&text=");
            stringBuilder.Append(HttpUtility.UrlEncode(text));
            var stringUrl = stringBuilder.ToString();
            // Console.WriteLine(stringUrl);
            return stringUrl;
        }


        public async Task DoPost(string text)
        {
            var client = new HttpClient();
            var request = await client.GetAsync(BuildUrl(text));
        }
    }
}