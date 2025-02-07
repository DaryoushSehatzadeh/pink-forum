using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinkForum.Models;

namespace PinkForum.Data
{
    public class PinkForumContext : DbContext
    {
        public PinkForumContext (DbContextOptions<PinkForumContext> options)
            : base(options)
        {
        }

        public DbSet<PinkForum.Models.Discussion> Discussion { get; set; } = default!;
        public DbSet<PinkForum.Models.Comment> Comment { get; set; } = default!;
    }
}
