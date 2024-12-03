using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using Data.Model;

namespace Data
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        // Default constructor for design-time support
        public ApplicationDBContext() { }
 

        public DbSet<StateInfo> StateInfos { get; set; }
        public DbSet<AdInfo> AdInfos { get; set; }
        public DbSet<AdDetailInfo> AdDetailInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", true, true);
            //IConfigurationRoot configuration = builder.Build();
            //optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=BaoNail_BackEnd_API_v2;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tắt cascade delete cho các mối quan hệ
            modelBuilder.Entity<AdDetailInfo>()
                .HasOne(ad => ad.StateInfo)
                .WithMany(state => state.AdDetailInfos)
                .HasForeignKey(ad => ad.StateInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdInfo>()
                .HasOne(ad => ad.StateInfo)
                .WithMany(state => state.AdInfos)
                .HasForeignKey(ad => ad.StateInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdDetailInfo>()
                .HasOne(adDetail => adDetail.AdInfo)
                .WithOne(ad => ad.AdDetailInfo)
                .HasForeignKey<AdDetailInfo>(adDetail => adDetail.AdInfoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
