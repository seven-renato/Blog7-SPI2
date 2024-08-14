using Blog7.Core.Abstractions;
using Blog7.Core.Authentication;
using Blog7.Data;
using Blog7.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog7.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserController(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    
    // GET: /User/
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("Username,Password")] User user)
    {
        if (!ModelState.IsValid)
            return View("Index", user);
        // Verifica se o usuário existe
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username); // Nome de usuário não encontrado
        if (existingUser == null)
        {
            ModelState.AddModelError("", "Username or password is incorrect");
            return View("Index", user);
        }
        // Verifica a senha
        var result = _passwordHasher.Verify(existingUser.Password, user.Password);// Senha incorreta
        if (!result)
        {
            ModelState.AddModelError("", "Username or password is incorrect");
            return View("Index", user);
        }

        var sessionId = Guid.NewGuid().ToString();
        Response.Cookies.Append("SessionId", sessionId, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
        
        SessionStore.AddSession(sessionId, existingUser.Id);
        
        return RedirectToAction("Index", "Post"); // Redireciona para a página inicial ou outra página protegida
    }

    public IActionResult RegisterUser()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Username,Password")] User user)
    {
        if (ModelState.IsValid)
        {
            var passwordHash = _passwordHasher.Hash(user.Password);
            user.Password = passwordHash;
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }
        return View("RegisterUser", user);
    }
}