using System.Text.RegularExpressions;

namespace VStore.Controllers;

[Authorize]
public class CartController(AppDbContext db, ILocalizer loc) : BaseController(db)
{
    public async Task<IActionResult> Index()
    {
        var uid = CurrentUserId!.Value;
        var items = await Db.CartItems.Include(c => c.Game).Where(c => c.UserId == uid).ToListAsync();
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int gameId, string? returnUrl)
    {
        var uid = CurrentUserId!.Value;
        var exists = await Db.Games.AnyAsync(g => g.Id == gameId);
        var owned = await Db.LibraryItems.AnyAsync(l => l.UserId == uid && l.GameId == gameId);
        var inCart = await Db.CartItems.AnyAsync(c => c.UserId == uid && c.GameId == gameId);
        if (exists && !owned && !inCart)
        {
            Db.CartItems.Add(new CartItem { UserId = uid, GameId = gameId });
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var uid = CurrentUserId!.Value;
        var item = await Db.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == uid);
        if (item != null)
        {
            Db.CartItems.Remove(item);
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> MoveToWishlist(int id)
    {
        var uid = CurrentUserId!.Value;
        var item = await Db.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == uid);
        if (item != null)
        {
            if (!await Db.WishlistItems.AnyAsync(w => w.UserId == uid && w.GameId == item.GameId))
                Db.WishlistItems.Add(new WishlistItem { UserId = uid, GameId = item.GameId });
            Db.CartItems.Remove(item);
            await Db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var uid = CurrentUserId!.Value;
        var items = await Db.CartItems.Include(c => c.Game).Where(c => c.UserId == uid).ToListAsync();
        if (items.Count == 0) return RedirectToAction(nameof(Index));
        return View(new CheckoutViewModel { Items = items, Total = items.Sum(i => i.Game.FinalPrice) });
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel m)
    {
        var uid = CurrentUserId!.Value;
        var items = await Db.CartItems.Include(c => c.Game).Where(c => c.UserId == uid).ToListAsync();
        if (items.Count == 0) return RedirectToAction(nameof(Index));
        m.Items = items;
        m.Total = items.Sum(i => i.Game.FinalPrice);

        if (m.Method == "card" && m.Total > 0)
        {
            var digits = new string((m.CardNumber ?? "").Where(char.IsDigit).ToArray());
            if (digits.Length != 16) ModelState.AddModelError(nameof(m.CardNumber), loc["card_number_invalid"]);
            if (string.IsNullOrWhiteSpace(m.Holder)) ModelState.AddModelError(nameof(m.Holder), loc["card_holder_required"]);
            if (!Regex.IsMatch(m.Expiry ?? "", @"^(0[1-9]|1[0-2])/\d{2}$")) ModelState.AddModelError(nameof(m.Expiry), loc["card_expiry_invalid"]);
            if (!Regex.IsMatch(m.Cvv ?? "", @"^\d{3}$")) ModelState.AddModelError(nameof(m.Cvv), loc["card_cvv_invalid"]);
        }
        if (!ModelState.IsValid) return View(m);

        var order = new Order
        {
            UserId = uid,
            Total = m.Total,
            PaymentMethod = m.Method == "card" ? "Банковская карта" : "Кошелёк V",
            Items = items.Select(i => new OrderItem { GameId = i.GameId, Price = i.Game.FinalPrice }).ToList()
        };
        Db.Orders.Add(order);

        foreach (var i in items)
        {
            Db.LibraryItems.Add(new LibraryItem { UserId = uid, GameId = i.GameId });
            var ach = await Db.Achievements.Where(a => a.GameId == i.GameId).OrderByDescending(a => a.Percent).FirstOrDefaultAsync();
            if (ach != null && !await Db.UserAchievements.AnyAsync(x => x.UserId == uid && x.AchievementId == ach.Id))
                Db.UserAchievements.Add(new UserAchievement { UserId = uid, AchievementId = ach.Id });
        }

        var ids = items.Select(i => i.GameId).ToList();
        Db.WishlistItems.RemoveRange(Db.WishlistItems.Where(w => w.UserId == uid && ids.Contains(w.GameId)));
        Db.CartItems.RemoveRange(items);
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success() => View();
}
