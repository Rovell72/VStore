using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace VStore.Models;

public static class Ext
{
    public static string Money(this decimal v) =>
        v == 0 ? "Бесплатно" : v.ToString("0.00", CultureInfo.InvariantCulture) + " $";
}

public class User
{
    public int Id { get; set; }
    [MaxLength(50)] public string Nickname { get; set; } = "";
    [MaxLength(200)] public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [MaxLength(20)] public string Role { get; set; } = "User";
    public bool IsBlocked { get; set; }
    [MaxLength(100)] public string? GoogleId { get; set; }
    [MaxLength(100)] public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<LibraryItem> Library { get; set; } = new();
    public List<UserAchievement> Achievements { get; set; } = new();
}

public class Game
{
    public int Id { get; set; }
    [MaxLength(200)] public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    [MaxLength(100)] public string Developer { get; set; } = "";
    [MaxLength(100)] public string Publisher { get; set; } = "";
    [MaxLength(50)] public string Genre { get; set; } = "";
    public DateTime ReleaseDate { get; set; }
    public decimal Price { get; set; }
    public int DiscountPercent { get; set; }
    public int AgeRating { get; set; }
    [MaxLength(20)] public string ColorFrom { get; set; } = "#2b5876";
    [MaxLength(20)] public string ColorTo { get; set; } = "#4e4376";
    public string MinRequirements { get; set; } = "";
    public string RecRequirements { get; set; } = "";
    public bool IsFeatured { get; set; }
    [NotMapped] public decimal FinalPrice => Math.Round(Price * (100 - DiscountPercent) / 100m, 2);
    public List<Achievement> Achievements { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<LibraryItem> Owners { get; set; } = new();
}

public class Achievement
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    [MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(300)] public string Description { get; set; } = "";
    [MaxLength(10)] public string Icon { get; set; } = "🏆";
    public int Percent { get; set; }
}

public class UserAchievement
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
}

public class Review
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsPositive { get; set; }
    [MaxLength(2000)] public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
}

public class WishlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

public class LibraryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public double HoursPlayed { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Total { get; set; }
    [MaxLength(50)] public string PaymentMethod { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int GameId { get; set; }
    public decimal Price { get; set; }
}

public class NewsItem
{
    public int Id { get; set; }
    [MaxLength(300)] public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(20)] public string ColorFrom { get; set; } = "#232526";
    [MaxLength(20)] public string ColorTo { get; set; } = "#414345";
}

public class FaqItem
{
    public int Id { get; set; }
    [MaxLength(200)] public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}

public class SupportTicket
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(200)] public string Email { get; set; } = "";
    [MaxLength(200)] public string Subject { get; set; } = "";
    [MaxLength(2000)] public string Message { get; set; } = "";
    [MaxLength(20)] public string Status { get; set; } = "Открыт";
    public string? AdminReply { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RepliedAt { get; set; }
}
