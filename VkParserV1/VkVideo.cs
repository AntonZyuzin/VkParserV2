namespace NetCore.Docker
{
    public class VkVideo
    {
        public string PreviewUrl { get; }
        public string VideoUrl { get; }
        
        public VkVideo(string previewUrl, string videoUrl)
        {
            PreviewUrl = previewUrl;
            VideoUrl = videoUrl;
        }
    }
}