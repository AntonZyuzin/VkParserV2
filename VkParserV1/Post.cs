namespace NetCore.Docker
{
    public class Post
    {
        public string Id { get; }
        public string Text { get; }
        public string Image { get; }

        public Post(string id, string text, string image)
        {
            Id = id;
            Text = text;
            Image = image;
        }
    }
}