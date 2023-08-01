using System;
using Microsoft.EntityFrameworkCore;

namespace demo_minimal_api
{
	public class PgDbContext : DbContext, IAppDbContext
    {
        public DbSet<Article> Articles { get; set; }

        public PgDbContext(DbContextOptions<MsSqlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Article>(entity => {
                entity.ToTable("Article");
                entity.Property(z => z.Id).HasColumnName("id");
                entity.Property(z => z.Title).HasColumnName("title");
                entity.Property(z => z.Url).HasColumnName("url");
                entity.Property(z => z.Content).HasColumnName("content");
                entity.Property(z => z.CreatedDate).HasColumnName("createdDate");
                entity.Property(z => z.CreatedBy).HasColumnName("createdBy");
                entity.Property(z => z.UpdatedDate).HasColumnName("updatedDate");
            });
        }
    }
}

