using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PinkForum.Data;
using PinkForum.Models;

namespace PinkForum.Controllers
{
    public class CommentsController : Controller
    {
        private readonly PinkForumContext _context;

        public CommentsController(PinkForumContext context)
        {
            _context = context;
        }

        // Deleted:
        // GET: Comments/Details/5
        // GET: Comments/Delete/5
        // POST: Comments/Delete/5
        // GET: Comments/Edit/5
        // POST: Comments/Edit/5

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }

        // GET: Comments/Create
        public IActionResult Create(int? discussionId)
        {
            if (discussionId == null)
            {
                return NotFound();
            }

            ViewData["DiscussionId"] = discussionId;

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content,CreateDate,DiscussionId")] Comment comment)
        {

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("DiscussionDetails", "Home", new { id = comment.DiscussionId });
            }

            return View();
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
