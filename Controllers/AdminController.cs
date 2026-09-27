namespace VStore.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext db) : BaseController(db)
{
    public async Task<IActionResult> Index()
    {
        ViewBag.UsersCount = await Db.Users.CountAsync();
        ViewBag.GamesCount = await Db.Games.CountAsync();
        ViewBag.OrdersCount = await Db.Orders.CountAsync();
        ViewBag.OpenTickets = await Db.Tickets.CountAsync(t => t.Status == "Открыт");
        ViewBag.Revenue = await Db.Orders.SumAsync(o => (decimal?)o.Total) ?? 0;
        return View();
    }

    public async Task<IActionResult> Games() =>
        View(await Db.Games.OrderByDescending(g => g.Id).ToListAsync());

    public IActionResult CreateGame() => View("EditGame", new AdminGameViewModel());

    public async Task<IActionResult> EditGame(int id)
    {
        var g = await Db.Games.FindAsync(id);
        if (g == null) return NotFound();
        return View(new AdminGameViewModel
        {
            Id = g.Id, Title = g.Title, Description = g.Description, Developer = g.Developer,
            Publisher = g.Publisher, Genre = g.Genre, ReleaseDate = g.ReleaseDate, Price = g.Price,
            DiscountPercent = g.DiscountPercent, AgeRating = g.AgeRating, ColorFrom = g.ColorFrom,
            ColorTo = g.ColorTo, MinRequirements = g.MinRequirements, RecRequirements = g.RecRequirements,
            IsFeatured = g.IsFeatured
        });
    }

    [HttpPost]
    public async Task<IActionResult> SaveGame(AdminGameViewModel m)
    {
        if (!ModelState.IsValid) return View("EditGame", m);

        Game g;
        if (m.Id == 0)
        {
            g = new Game();
            Db.Games.Add(g);
        }
        else
        {
            var existing = await Db.Games.FindAsync(m.Id);
            if (existing == null) return NotFound();
            g = existing;
        }

        g.Title = m.Title.Trim();
        g.Description = m.Description.Trim();
        g.Developer = m.Developer.Trim();
        g.Publisher = m.Publisher.Trim();
        g.Genre = m.Genre.Trim();
        g.ReleaseDate = m.ReleaseDate;
        g.Price = m.Price;
        g.DiscountPercent = m.DiscountPercent;
        g.AgeRating = m.AgeRating;
        g.ColorFrom = m.ColorFrom;
        g.ColorTo = m.ColorTo;
        g.MinRequirements = m.MinRequirements;
        g.RecRequirements = m.RecRequirements;
        g.IsFeatured = m.IsFeatured;

        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Games));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var g = await Db.Games.FindAsync(id);
        if (g != null)
        {
            Db.Games.Remove(g);
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Games));
    }

    public async Task<IActionResult> Users() =>
        View(await Db.Users.OrderBy(u => u.Nickname).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> ToggleBlock(int id)
    {
        var u = await Db.Users.FindAsync(id);
        if (u != null && u.Role != "Admin")
        {
            u.IsBlocked = !u.IsBlocked;
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Tickets() =>
        View(await Db.Tickets.OrderByDescending(t => t.CreatedAt).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> ReplyTicket(int id, string reply)
    {
        var t = await Db.Tickets.FindAsync(id);
        if (t != null && !string.IsNullOrWhiteSpace(reply))
        {
            t.AdminReply = reply.Trim();
            t.Status = "Отвечено";
            t.RepliedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Tickets));
    }
}
