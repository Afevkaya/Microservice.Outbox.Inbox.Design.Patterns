using Microsoft.EntityFrameworkCore;
using Stock.Service.Models.Entities;

namespace Stock.Service.Contexts;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base( options)
    {
        
    }

    public DbSet<OrderInbox> OrderInboxes { get; set; }
}