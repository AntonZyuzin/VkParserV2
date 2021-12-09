using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace NetCore.Docker
{
    public class VkPostBuilder
    {
        
        private readonly string _databaseFilePath; // path to 'posts.txt'

        public VkPostBuilder(string filePath)
        {
            _databaseFilePath = filePath;
        }

        public List<Post> Save(List<string> listId, List<string> listText, List<string> listImages, List<VkVideo> listVideos)
        {
            var listOfPosts = new List<Post>();
            if (listOfPosts == null) throw new ArgumentNullException(nameof(listOfPosts));
            if (listId == null || listText == null) return null;
            listOfPosts.AddRange(listId.Select((t, index) =>
                new Post(t, listText[index], listImages[index], listVideos[index])));
            File.AppendAllLines(_databaseFilePath, listOfPosts.Select(_ => _.Id));
            return listOfPosts;
        }

        public List<Post> SaveWithTag(List<string> postIds, List<string> texts, List<string> images,
            List<VkVideo> videos, string tag)
        {
            var posts = postIds.Select((t, i) => new Post(t, texts[i], images[i], videos[i]))
                               .Where((post, i) => texts[i].Contains(tag)
                                                   && CompareIdInFile(post.Id))
                               .ToList();
            return posts;
        }


        private bool CompareIdInFile(string id)
        {
            string[] fileStrings = File.ReadAllLines(_databaseFilePath);
            return !fileStrings.Contains(id);
        }
    }
}