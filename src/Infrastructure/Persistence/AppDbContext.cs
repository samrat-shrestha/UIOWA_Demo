using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Receipt> Receipts => Set<Receipt>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Receipt>(b =>
        {
            b.ToTable("Receipts");
            b.HasKey(r => r.Id);
            b.Property(r => r.Amount).HasColumnType("decimal(10,2)");
            b.HasIndex(r => new { r.PurchaseDate, r.Amount });
        });
    }
}