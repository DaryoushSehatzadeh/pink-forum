using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PinkForum.Models
{
    public class Comment
    {

        public int CommentId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public int DiscussionId { get; set; }

        public Discussion? Discussion { get; set; }
    }
}
