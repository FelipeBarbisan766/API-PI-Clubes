using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<ClubAdmin> ClubAdmins { get; set; }
        public DbSet<Image> Images { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}
