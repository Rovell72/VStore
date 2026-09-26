namespace VStore.Controllers;

[Authorize]
public class WishlistController(AppDbContext db) : BaseController(db)
{
    public async Task<IActionResult> Index()
    {
        var uid = CurrentUserId!.Value;
        var items = await Db.WishlistItems.Include(w => w.Game).Where(w => w.UserId == uid).OrderByDescending(w => w.AddedAt).ToListAsync();
        ViewBag.InCart = await Db.CartItems.Where(c => c.UserId == uid).Select(c => c.GameId).ToListAsync();
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int gameId, string? returnUrl)
    {
        var uid = CurrentUserId!.Value;
        var exists = await Db.Games.AnyAsync(g => g.Id == gameId);
        var owned = await Db.LibraryItems.AnyAsync(l => l.UserId == uid && l.GameId == gameId);
        var already = await Db.WishlistItems.AnyAsync(w => w.UserId == uid && w.GameId == gameId);
        if (exists && !owned && !already)
        {
            Db.WishlistItems.Add(new WishlistItem { UserId = uid, GameId = gameId });
            await Db.SaveChangesAsync();
        }
        return BackOr(returnUrl, "Index", "Wishlist");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id, string? returnUrl)
    {
        var uid = CurrentUserId!.Value;
        var item = await Db.WishlistItems.FirstOrDefaultAsync(w => w.Id == id && w.UserId == uid);
        if (item != null)
        {
            Db.WishlistItems.Remove(item);
            await Db.SaveChangesAsync();
        }
        return BackOr(returnUrl, "Index", "Wishlist");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveByGame(int gameId, string? returnUrl)
    {
        var uid = CurrentUserId!.Value;
        var item = await Db.WishlistItems.FirstOrDefaultAsync(w => w.GameId == gameId && w.UserId == uid);
        if (item != null)
        {
            Db.WishlistItems.Remove(item);
            await Db.SaveChangesAsync();
        }
        return BackOr(returnUrl, "Index", "Wishlist");
    }

    [HttpPost]
    public async Task<IActionResult> ToCart(int id)
    {
        var uid = CurrentUserId!.Value;
        var item = await Db.WishlistItems.FirstOrDefaultAsync(w => w.Id == id && w.UserId == uid);
        if (item != null)
        {
            if (!await Db.CartItems.AnyAsync(c => c.UserId == uid && c.GameId == item.GameId))
                Db.CartItems.Add(new CartItem { UserId = uid, GameId = item.GameId });
            Db.WishlistItems.Remove(item);
            await Db.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Cart");
    }
}
