using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PinkForum.Data
{
    public class ApplicationUser : IdentityUser
    {

        [PersonalData]
        public required string Name { get; set; } = string.Empty;

        [PersonalData]
        public string Location { get; set; } = string.Empty;

        [PersonalData]
        public string ImageFilename { get; set; } = string.Empty;

        [PersonalData]
        [NotMapped]
        public IFormFile? Image { get; set; }

    }
}
