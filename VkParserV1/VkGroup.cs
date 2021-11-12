namespace NetCore.Docker
{
    public class VkGroup
    {
        public string GroupName { get; }
        public int CountOfPosts { get; }
        public string Token { get; }

        public VkGroup(string groupName, int countOfPosts, string token)
        {
            GroupName = groupName;
            CountOfPosts = countOfPosts;
            Token = token;
        }
    }
}