namespace VStore.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<NewsItem> News => Set<NewsItem>();
    public DbSet<FaqItem> Faq => Set<FaqItem>();
    public DbSet<SupportTicket> Tickets => Set<SupportTicket>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserAchievement>().HasKey(x => new { x.UserId, x.AchievementId });
        b.Entity<UserAchievement>().HasOne(x => x.Achievement).WithMany().HasForeignKey(x => x.AchievementId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<User>().HasIndex(x => x.Nickname).IsUnique();
        b.Entity<User>().HasIndex(x => x.GoogleId).IsUnique(false);
        b.Entity<Game>().Property(x => x.Price).HasPrecision(10, 2);
        b.Entity<Order>().Property(x => x.Total).HasPrecision(10, 2);
        b.Entity<OrderItem>().Property(x => x.Price).HasPrecision(10, 2);
        b.Entity<CartItem>().HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        b.Entity<WishlistItem>().HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        b.Entity<LibraryItem>().HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        b.Entity<Review>().HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        b.Entity<Review>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<LibraryItem>().HasOne(x => x.User).WithMany(u => u.Library).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<LibraryItem>().HasOne(x => x.Game).WithMany(g => g.Owners).HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<CartItem>().HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<WishlistItem>().HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Restrict);
    }
}
