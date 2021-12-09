using System.Collections.Generic;

namespace NetCore.Docker
{
    public class Post
    {
        public string Id { get; }
        public string Text { get; }
        public string Image { get; }
        public VkVideo Video { get; }

        public Post(string id, string text, string image, VkVideo video)
        {
            Id = id;
            Text = text;
            Image = image;
            Video = video;
        }
    }
}