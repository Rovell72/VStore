using System.ComponentModel.DataAnnotations;

namespace VStore.Models;

public class HomeViewModel
{
    public string? Query { get; set; }
    public string? Genre { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<Game> Featured { get; set; } = new();
    public List<Game> Specials { get; set; } = new();
    public List<Game> NewReleases { get; set; } = new();
    public List<Game> Free { get; set; } = new();
    public List<Game> TopSellers { get; set; } = new();
    public List<Game> Results { get; set; } = new();
    public bool IsFiltered => !string.IsNullOrWhiteSpace(Query) || !string.IsNullOrWhiteSpace(Genre);
}

public class GameViewModel
{
    public Game Game { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();
    public int PositiveCount { get; set; }
    public bool InLibrary { get; set; }
    public bool InCart { get; set; }
    public bool InWishlist { get; set; }
    public bool IsAuthenticated { get; set; }
    public string ReviewSummary
    {
        get
        {
            if (Reviews.Count == 0) return "Нет отзывов";
            var p = PositiveCount * 100 / Reviews.Count;
            return p >= 80 ? "Очень положительные" : p >= 60 ? "В основном положительные" : p >= 40 ? "Смешанные" : "В основном отрицательные";
        }
    }
}

public class ProfileViewModel
{
    public User User { get; set; } = null!;
    public List<LibraryItem> Library { get; set; } = new();
    public List<Achievement> Unlocked { get; set; } = new();
    public int Level => 1 + Library.Count * 3 + Unlocked.Count;
}

public class AchievementRow
{
    public Achievement Achievement { get; set; } = null!;
    public bool Unlocked { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите игровое имя"), StringLength(30, MinimumLength = 3, ErrorMessage = "Имя должно содержать от 3 до 30 символов")]
    public string Nickname { get; set; } = "";
    [Required(ErrorMessage = "Введите почту"), EmailAddress(ErrorMessage = "Некорректный адрес почты")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Придумайте пароль"), StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; } = "";
    [Range(typeof(bool), "true", "true", ErrorMessage = "Необходимо принять условия соглашения")]
    public bool AcceptTerms { get; set; }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите имя аккаунта или почту")]
    public string Login { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class CheckoutViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string Method { get; set; } = "card";
    public string? CardNumber { get; set; }
    public string? Holder { get; set; }
    public string? Expiry { get; set; }
    public string? Cvv { get; set; }
}
