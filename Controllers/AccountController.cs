using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace VStore.Controllers;

public class AccountController(AppDbContext db, IPasswordHasher<User> hasher) : BaseController(db)
{
    private class QrTokenEntry
    {
        public string Token { get; set; } = null!;
        public int? UserId { get; set; }
        public DateTime Expires { get; set; }
    }
    private static readonly ConcurrentDictionary<string, QrTokenEntry> _qrTokens = new();

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        var token = Guid.NewGuid().ToString("N");
        _qrTokens[token] = new QrTokenEntry { Token = token, Expires = DateTime.UtcNow.AddMinutes(5) };
        return View(new LoginViewModel { ReturnUrl = returnUrl, QRToken = token });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel m)
    {
        if (!ModelState.IsValid) return View(m);
        var login = m.Login.Trim();
        var user = await Db.Users.FirstOrDefaultAsync(u => u.Email == login || u.Nickname == login);
        if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, m.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Неверное имя аккаунта или пароль");
            return View(m);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nickname)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = m.RememberMe });
        return BackOr(m.ReturnUrl, "Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel m)
    {
        if (!ModelState.IsValid) return View(m);
        var email = m.Email.Trim().ToLowerInvariant();
        var nick = m.Nickname.Trim();
        if (await Db.Users.AnyAsync(u => u.Email == email))
            ModelState.AddModelError(nameof(m.Email), "Пользователь с такой почтой уже существует");
        if (await Db.Users.AnyAsync(u => u.Nickname == nick))
            ModelState.AddModelError(nameof(m.Nickname), "Это игровое имя уже занято");
        if (!ModelState.IsValid) return View(m);

        var user = new User { Nickname = nick, Email = email };
        user.PasswordHash = hasher.HashPassword(user, m.Password);
        Db.Users.Add(user);
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Created));
    }

    public IActionResult Created() => View();

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var uid = CurrentUserId!.Value;
        var vm = new ProfileViewModel
        {
            User = await Db.Users.FirstAsync(u => u.Id == uid),
            Library = await Db.LibraryItems.Include(l => l.Game).Where(l => l.UserId == uid).OrderByDescending(l => l.HoursPlayed).ToListAsync(),
            Unlocked = await Db.Achievements.Include(a => a.Game).Where(a => Db.UserAchievements.Any(u => u.UserId == uid && u.AchievementId == a.Id)).OrderBy(a => a.Percent).ToListAsync()
        };
        return View(vm);
    }

    [Authorize]
    public async Task<IActionResult> Achievements()
    {
        var uid = CurrentUserId!.Value;
        var unlocked = await Db.UserAchievements.Where(a => a.UserId == uid).Select(a => a.AchievementId).ToListAsync();
        var all = await Db.Achievements.Include(a => a.Game).OrderBy(a => a.Game.Title).ThenByDescending(a => a.Percent).ToListAsync();
        var rows = all.Select(a => new AchievementRow { Achievement = a, Unlocked = unlocked.Contains(a.Id) })
                      .OrderByDescending(r => r.Unlocked).ToList();
        return View(rows);
    }

    [HttpGet]
    public IActionResult QrPoll(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || !_qrTokens.TryGetValue(token, out var entry) || entry.Expires < DateTime.UtcNow)
            return Json(new { authenticated = false });
        return Json(new { authenticated = entry.UserId.HasValue });
    }

    [HttpGet]
    public IActionResult QrScan(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || !_qrTokens.TryGetValue(token, out var entry) || entry.Expires < DateTime.UtcNow)
            return NotFound();
        return View(new LoginViewModel { QRToken = token });
    }

    [HttpPost]
    public async Task<IActionResult> QrScan(LoginViewModel m)
    {
        if (string.IsNullOrWhiteSpace(m.QRToken) || !_qrTokens.TryGetValue(m.QRToken, out var entry) || entry.Expires < DateTime.UtcNow)
            return NotFound();
        if (!ModelState.IsValid) return View(m);
        var login = m.Login.Trim();
        var user = await Db.Users.FirstOrDefaultAsync(u => u.Email == login || u.Nickname == login);
        if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, m.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Неверное имя аккаунта или пароль");
            return View(m);
        }
        // mark token as authenticated for this user
        entry.UserId = user.Id;
        _qrTokens[m.QRToken!] = entry;
        // show confirmation to scanner device
        return View("Created");
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> QrAutoLogin(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || !_qrTokens.TryGetValue(token, out var entry) || entry.Expires < DateTime.UtcNow || !entry.UserId.HasValue)
            return Json(new { ok = false });
        var user = await Db.Users.FirstOrDefaultAsync(u => u.Id == entry.UserId.Value);
        if (user == null) return Json(new { ok = false });

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nickname)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // consume token
        _qrTokens.TryRemove(token, out _);
        return Json(new { ok = true, redirect = Url.Action("Index", "Home") });
    }
}
