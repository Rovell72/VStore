namespace VStore.Controllers;

public abstract class BaseController(AppDbContext db) : Controller
{
    protected readonly AppDbContext Db = db;

    protected int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    protected IActionResult BackOr(string? returnUrl, string action, string controller) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(action, controller);
}
