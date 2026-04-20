using Microsoft.EntityFrameworkCore;
using backend.context.identity.domain.entity;
using backend.context.naildesign.domain.entity;
using backend.context.payment.domain.entity;
using backend.context.todo.domain.entity;
using backend.context.notification.domain.entity;

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
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<backend.context.nailservice.domain.entity.NailService> NailServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Tự động quét và áp dụng các cấu hình từ Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}