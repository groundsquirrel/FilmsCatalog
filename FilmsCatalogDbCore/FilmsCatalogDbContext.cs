using System;
using System.Collections.Generic;
using System.IO;
using FilmsCatalogModels.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FilmsCatalogDbCore
{
    public class FilmsCatalogDbContext : DbContext
    {
        private static IConfigurationRoot _configuration;

        public DbSet<Film> Films { get; set; }
        public DbSet<Director> Directors { get; set; }

        public FilmsCatalogDbContext() : base()
        {

        }

        public FilmsCatalogDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("FilmsCatalog");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilmDirector>()
                        .HasIndex(fd => new { fd.FilmId, fd.DirectorId })
                        .IsUnique()
                        .IsClustered(false);

            modelBuilder.Entity<Film>(cfg =>
            {
                cfg.HasQueryFilter(p => !p.IsDeleted);
                cfg.HasIndex(f => f.Name).IsClustered(false);
                cfg.HasIndex(f => f.Year).IsClustered(false);
            });

            modelBuilder.Entity<Director>(cfg =>
            {
                cfg.HasQueryFilter(p => !p.IsDeleted)
                   .HasIndex(d => d.Name)
                   .IsClustered(false);
            });
        }


    }
}
