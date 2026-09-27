namespace VStore.Controllers;

[Authorize]
public class LibraryController(AppDbContext db) : BaseController(db)
{
    public async Task<IActionResult> Index(string? q)
    {
        var uid = CurrentUserId!.Value;
        var query = Db.LibraryItems.Include(l => l.Game).Where(l => l.UserId == uid);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(l => l.Game.Title.Contains(q));
        var items = await query.OrderByDescending(l => l.HoursPlayed).ToListAsync();
        ViewBag.Query = q;
        return View(items);
    }
}
