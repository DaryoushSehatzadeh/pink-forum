using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PinkForum.Data;
using PinkForum.Models;

namespace PinkForum.Controllers
{
    [Authorize]
    public class DiscussionsController : Controller
    {
        private readonly PinkForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscussionsController(PinkForumContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discussions
        public async Task<IActionResult> Index()
        {
            var userID = _userManager.GetUserId(User);
            var discussions = await _context.Discussion
                .Include(m => m.ApplicationUser)
                .ToListAsync();

            List<Discussion> userDiscussions = new List<Discussion>();

            foreach (var discussion in discussions)
            {
                if (discussion.ApplicationUserId == userID)
                {
                    userDiscussions.Add(discussion);
                }
            }

            // Get all image filenames in the wwwroot/images directory
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var imageFiles = Directory.GetFiles(imagesDirectory).Select(Path.GetFileName).ToList();

            // Pass the image files to the view (or use them in your code)
            ViewBag.ImageFiles = imageFiles;

            return View(userDiscussions);
        }

        // GET: Discussions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion
                .Include(m => m.ApplicationUser)
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

        // GET: Discussions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discussions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,ImageFilename,CreateDate,ImageFile,ApplicationUserId")] Discussion discussion)
        {
            // rename the uploaded file to a guid (unique filename). Set before photo saved in database.
            discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile?.FileName);

            // Validation
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                discussion.ApplicationUserId = userID;

                _context.Add(discussion);
                await _context.SaveChangesAsync();

                // Save the uploaded file after the photo is saved in the database.
                if (discussion.ImageFile != null)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Re-direct to the Photos/Index page
                return RedirectToAction("DiscussionDetails", "Home", new { id = discussion.DiscussionId });
            }
            return View(discussion);
        }

        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion.FindAsync(id);
            if (discussion == null)
            {
                return NotFound();
            }
            return View(discussion);
        }

        // POST: Discussions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Discussions/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,Content,ImageFilename")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDiscussion = await _context.Discussion.FindAsync(id);
                    if (existingDiscussion == null)
                    {
                        return NotFound();
                    }

                    // Retain existing ApplicationUserId
                    discussion.ApplicationUserId = existingDiscussion.ApplicationUserId;

                    if (discussion.ImageFile != null) // If a new image is uploaded
                    {
                        // Save the new image
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", discussion.ImageFilename);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await discussion.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        // Keep the old image filename if no new file is uploaded
                        discussion.ImageFilename = existingDiscussion.ImageFilename;
                    }

                    _context.Entry(existingDiscussion).CurrentValues.SetValues(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(discussion); // Return the view if validation fails
        }



        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion
                .Include(m => m.ApplicationUser)
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

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussion.FindAsync(id);
            if (discussion != null)
            {
                _context.Discussion.Remove(discussion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussion.Any(e => e.DiscussionId == id);
        }
    }
}
