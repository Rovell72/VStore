namespace VStore.Controllers;

public class HomeController(AppDbContext db, IEmailService emailService, ILocalizer loc) : BaseController(db)
{
    public async Task<IActionResult> Index(string? q, string? genre)
    {
        var vm = new HomeViewModel
        {
            Query = q,
            Genre = genre,
            Genres = await Db.Games.Select(g => g.Genre).Distinct().OrderBy(g => g).ToListAsync()
        };

        if (vm.IsFiltered)
        {
            var query = Db.Games.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) query = query.Where(g => g.Title.Contains(q));
            if (!string.IsNullOrWhiteSpace(genre)) query = query.Where(g => g.Genre == genre);
            vm.Results = await query.OrderBy(g => g.Title).ToListAsync();
            return View(vm);
        }

        vm.Featured = await Db.Games.Where(g => g.IsFeatured).OrderByDescending(g => g.ReleaseDate).Take(4).ToListAsync();
        vm.Specials = await Db.Games.Where(g => g.DiscountPercent > 0).OrderByDescending(g => g.DiscountPercent).Take(6).ToListAsync();
        vm.NewReleases = await Db.Games.OrderByDescending(g => g.ReleaseDate).Take(6).ToListAsync();
        vm.Free = await Db.Games.Where(g => g.Price == 0).Take(6).ToListAsync();
        vm.TopSellers = await Db.Games.OrderByDescending(g => g.Owners.Count).ThenBy(g => g.Title).Take(5).ToListAsync();
        return View(vm);
    }

    public async Task<IActionResult> Game(int id)
    {
        var game = await Db.Games.Include(g => g.Achievements.OrderByDescending(a => a.Percent)).FirstOrDefaultAsync(g => g.Id == id);
        if (game == null) return NotFound();

        var reviews = await Db.Reviews.Include(r => r.User).Where(r => r.GameId == id).OrderByDescending(r => r.CreatedAt).ToListAsync();
        var vm = new GameViewModel
        {
            Game = game,
            Reviews = reviews,
            PositiveCount = reviews.Count(r => r.IsPositive),
            IsAuthenticated = CurrentUserId != null
        };
        if (CurrentUserId is int uid)
        {
            vm.InLibrary = await Db.LibraryItems.AnyAsync(l => l.UserId == uid && l.GameId == id);
            vm.InCart = await Db.CartItems.AnyAsync(c => c.UserId == uid && c.GameId == id);
            vm.InWishlist = await Db.WishlistItems.AnyAsync(w => w.UserId == uid && w.GameId == id);
        }
        return View(vm);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> AddReview(int gameId, bool positive, string? text)
    {
        var uid = CurrentUserId!.Value;
        if (!await Db.LibraryItems.AnyAsync(l => l.UserId == uid && l.GameId == gameId))
            return RedirectToAction(nameof(Game), new { id = gameId });
        if (string.IsNullOrWhiteSpace(text))
            return RedirectToAction(nameof(Game), new { id = gameId });

        var review = await Db.Reviews.FirstOrDefaultAsync(r => r.UserId == uid && r.GameId == gameId);
        if (review == null)
        {
            review = new Review { UserId = uid, GameId = gameId };
            Db.Reviews.Add(review);
        }
        review.IsPositive = positive;
        review.Text = text.Trim().Length > 2000 ? text.Trim()[..2000] : text.Trim();
        review.CreatedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        return RedirectToAction(nameof(Game), new { id = gameId });
    }

    public async Task<IActionResult> News() =>
        View(await Db.News.OrderByDescending(n => n.PublishedAt).ToListAsync());

    public async Task<IActionResult> Support()
    {
        var vm = new SupportPageViewModel { Faq = await Db.Faq.OrderBy(f => f.Id).ToListAsync() };
        if (CurrentUserId is int uid)
        {
            var user = await Db.Users.FindAsync(uid);
            if (user != null) vm.Ticket = new SupportTicketViewModel { Name = user.Nickname, Email = user.Email };
        }
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitTicket(SupportTicketViewModel Ticket)
    {
        var vm = new SupportPageViewModel { Faq = await Db.Faq.OrderBy(f => f.Id).ToListAsync(), Ticket = Ticket };
        if (!ModelState.IsValid)
        {
            vm.ShowForm = true;
            return View("Support", vm);
        }

        var name = Ticket.Name.Trim();
        var email = Ticket.Email.Trim();
        var subject = Ticket.Subject.Trim();
        var message = Ticket.Message.Trim();

        try
        {
            await emailService.SendSupportTicketAsync(Ticket);
        }
        catch
        {
            ModelState.AddModelError("", loc["email_send_failed"]);
            vm.ShowForm = true;
            return View("Support", vm);
        }

        Db.Tickets.Add(new SupportTicket
        {
            UserId = CurrentUserId,
            Name = name,
            Email = email,
            Subject = subject,
            Message = message
        });
        await Db.SaveChangesAsync();
        vm.TicketSent = true;
        vm.Ticket = new SupportTicketViewModel();
        return View("Support", vm);
    }
}
