using System;
using Microsoft.EntityFrameworkCore;

namespace demo_minimal_api
{
	public class MsSqlDbContext : DbContext, IAppDbContext
    {
        public DbSet<Article> Articles { get; set; }

        public MsSqlDbContext(DbContextOptions<MsSqlDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Article>(entity => {
                entity.ToTable("Article");
                entity.Property(z => z.Id).HasColumnName("Id");
                entity.Property(z => z.Title).HasColumnName("Title");
                entity.Property(z => z.Url).HasColumnName("Url");
                entity.Property(z => z.Content).HasColumnName("Content");
                entity.Property(z => z.CreatedDate).HasColumnName("CreatedDate");
                entity.Property(z => z.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(z => z.UpdatedDate).HasColumnName("UpdatedDate");
            });
        }
    }
}
