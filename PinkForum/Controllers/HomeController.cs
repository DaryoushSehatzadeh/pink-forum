using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PinkForum.Data;
using PinkForum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace PinkForum.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly PinkForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(PinkForumContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // get all discussions
            var discussions = await _context.Discussion
                .Include(m => m.ApplicationUser)
                .Include(m => m.Comments)
                .ToListAsync();

            // Get all image filenames in the wwwroot/images directory
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var imageFiles = Directory.GetFiles(imagesDirectory).Select(Path.GetFileName).ToList();

            // Pass the image files to the view (or use them in your code)
            ViewBag.ImageFiles = imageFiles;

            return View(discussions);
        }

        // Get discussion details by id
        public async Task<IActionResult> DiscussionDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get discussion by id
            var discussion = await _context.Discussion
                .Include(m => m.ApplicationUser)
                .Include(m => m.Comments)
                    .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            // Get all image filenames in the wwwroot/images directory
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var imageFiles = Directory.GetFiles(imagesDirectory).Select(Path.GetFileName).ToList();

            // Pass the image files to the view (or use them in your code)
            ViewBag.ImageFiles = imageFiles;

            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        public async Task<IActionResult> Profile(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            var discussions = await _context.Discussion
                .Include(m => m.Comments)
                    .ThenInclude(c => c.ApplicationUser)
                .Include(m => m.ApplicationUser)
                .Where(d => d.ApplicationUserId == id)
                .ToListAsync();

            ViewBag.user = user;

            return View(discussions);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
