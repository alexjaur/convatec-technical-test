using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence
{
    public class LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) : DbContext(options)
    {
        public DbSet<Transporter> Transporters => Set<Transporter>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Transporter>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(120).IsRequired();
                e.Property(x => x.Document).HasMaxLength(30).IsRequired();
                e.Property(x => x.Email).HasMaxLength(120).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.Document).IsUnique();
            });
        }
    }
}
