using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog7.Core.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog7.Data;
using Blog7.Models;

namespace Blog7.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            if (Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                if (SessionStore.TryGetUserId(sessionId, out var userId))
                {
                    // Sessão válida, você pode recuperar o usuário associado e passar para a view
                    var user = await _context.Users.FindAsync(userId);
                    ViewBag.CurrentUser = user;
                }
                else
                {
                    // Sessão inválida, redirecionar para login
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                // Cookie não encontrado, redirecionar para login
                return RedirectToAction("Index", "User");
            }

            var posts = _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.TimeStamp);

            return View(await posts.ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public async Task<IActionResult> Create()
        {
            if (Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                if (SessionStore.TryGetUserId(sessionId, out var userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    ViewBag.CurrentUser = user;
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Post post)
        {
            if (Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                if (SessionStore.TryGetUserId(sessionId, out var userId))
                {
                    var user = await _context.Users.FindAsync(userId);

                    if (user == null)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    
                    post.UserId = userId;
                    post.TimeStamp = DateTime.UtcNow;
                    
                    if (string.IsNullOrWhiteSpace(post.Title))
                    {
                        ModelState.AddModelError("Title", "Title is required.");
                        return View("Create", post);
                    }

                    if (string.IsNullOrWhiteSpace(post.Content))
                    {
                        ModelState.AddModelError("Content", "Content is required.");
                        return View("Create", post);
                    }
                    
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }
        
        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Content")] Post post)
        {
            if (Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                if (SessionStore.TryGetUserId(sessionId, out var userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    ViewBag.CurrentUser = user;
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var lastPost = await _context.Posts.FindAsync(id);
                            post.UserId = user.Id;
                            post.TimeStamp = lastPost.TimeStamp;
                            if (string.IsNullOrWhiteSpace(post.Title))
                            {
                                ModelState.AddModelError("Title", "Title is required.");
                                return View("Create", post);
                            }

                            if (string.IsNullOrWhiteSpace(post.Content))
                            {
                                ModelState.AddModelError("Content", "Content is required.");
                                return View("Create", post);
                            }
                            _context.Update(post);
                            _context.Remove(lastPost);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PostExists(post.Id))
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
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
