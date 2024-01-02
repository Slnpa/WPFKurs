using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.Repositories
{
    public class MobileDbContext : DbContext
    {
        public DbSet<Mobile> Mobiles { get; set; }
        public DbSet<MobileInBasket> Basket { get; set; }
        public DbSet<MobileInFavorites> Favorites { get; set; }
        public DbSet<Ordert> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=123;Database=kursach;");
        }
    }

}
