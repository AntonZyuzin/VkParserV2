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
            return
                $"https://api.telegram.org/bot{HttpUtility.UrlEncode(_token)}/sendMessage?chat_id={HttpUtility.UrlEncode(_chanelId)}&text={text}";
        }


        public async Task DoPost(string text)
        {
            var client = new HttpClient();
            var request = await client.GetAsync(BuildUrl(text));
            Console.WriteLine("пост сделан");
        }
    }
}