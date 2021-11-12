using System.Collections.Generic;

namespace NetCore.Docker
{
    public class Database
    {
        public List<Post> ListPosts;

        public void CleanList()
        {
            if (ListPosts.Count > 100)
            {
                for (var i = 0; i < 50; i++)
                {
                    ListPosts.Remove(ListPosts[i]);
                }
            }
        }
    }
}