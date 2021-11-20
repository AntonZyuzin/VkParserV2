using System.Collections.Generic;

namespace NetCore.Docker
{
    public class Database
    {
        public List<Post> ListPosts { get; } = new List<Post>();
        public void CleanList()
        {
            if (ListPosts.Count > 100)
            { 
                ListPosts.RemoveRange(0, 50);
            }
        }
    }
}