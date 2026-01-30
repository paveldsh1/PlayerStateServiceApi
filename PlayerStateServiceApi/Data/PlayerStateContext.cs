using Microsoft.EntityFrameworkCore;
using PlayerStateServiceApi.Models;

namespace PlayerStateServiceApi.Data
{
    internal class PlayerStateContext : DbContext
    {
        public PlayerStateContext(DbContextOptions<PlayerStateContext> options) : base(options) { }

        public DbSet<PlayerState> PlayerStates => Set<PlayerState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerState>(b =>
            {
                b.HasKey(x => x.Nickname);
                b.Property(x => x.Nickname).IsRequired();
                b.Property(x => x.PlayerX).IsRequired();
                b.Property(x => x.PlayerY).IsRequired();
                b.Property(x => x.PlayerA).IsRequired();
                b.Property(x => x.UpdatedAt).IsRequired();
            });
        }
    }
}
