using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PinkForum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PinkForum.Data
{
    public class PinkForumContext : IdentityDbContext
    {
        public PinkForumContext (DbContextOptions<PinkForumContext> options)
            : base(options)
        {
        }

        public DbSet<PinkForum.Models.Discussion> Discussion { get; set; } = default!;
        public DbSet<PinkForum.Models.Comment> Comment { get; set; } = default!;
    }
}
