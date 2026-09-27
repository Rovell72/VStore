namespace VStore.Controllers;

public class SettingsController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string lang, string? returnUrl)
    {
        if (Localizer.Langs.Contains(lang))
            Response.Cookies.Append("lang", lang, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        return LocalRedirectSafe(returnUrl);
    }

    [HttpPost]
    public IActionResult SetTheme(string theme, string? returnUrl)
    {
        if (theme is "dark" or "light")
            Response.Cookies.Append("theme", theme, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        return LocalRedirectSafe(returnUrl);
    }

    IActionResult LocalRedirectSafe(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Home");
}
