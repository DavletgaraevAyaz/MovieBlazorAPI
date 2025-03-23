using APIBlazor.Model;
using Microsoft.EntityFrameworkCore;

namespace APIBlazor.DataBaseContext
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions options) : base(options) 
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Logins>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Movie>()
                .Property(m => m.Rating)
                .HasPrecision(3, 1);
        }      
        public DbSet<Users> Users { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Role> Role {  get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<ChatFilm> ChatFilm { get; set; }
    }
}
