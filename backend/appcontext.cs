using Microsoft.EntityFrameworkCore;
using backend.context.identity.domain.entity;
using backend.context.naildesign.domain.entity;
using backend.context.payment.domain.entity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<backend.context.booking.domain.entity.Booking> Bookings { get; set; }
    public DbSet<NailDesign> NailDesigns { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Tự động quét và áp dụng các cấu hình từ Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}