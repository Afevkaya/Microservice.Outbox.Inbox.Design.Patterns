using Microsoft.EntityFrameworkCore;

namespace Order.Api.Contexts;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base( options)
    {
        
    }

    public DbSet<Models.Entities.Order> Orders { get; set; }
    public DbSet<Models.Entities.OrderItem> OrderItems { get; set; }
    public DbSet<Models.Entities.OrderOutbox> OrderOutboxes { get; set; }
}