using Microsoft.EntityFrameworkCore;

namespace AmarenderReddy.Data
{
    public class DummyContext : DbContext
    {
        public DbSet<DummyModel> DummyModel { get; set; }

        public DummyContext(DbContextOptions<DummyContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=dummy;Username=postgres;Password=Password");
    }
}
