using HNG.StageTwoTask.BackendC_.Models.Access;
using HNG.StageTwoTask.BackendC_.Models.Organisation;
using Microsoft.EntityFrameworkCore;

namespace HNG.StageTwoTask.BackendC_.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<Organisations>(entity =>
            {
                entity.HasKey(e => e.OrgId);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<OrganisationUser>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.OrgId });
                entity.HasOne(e => e.User).WithMany(u => u.OrganisationUser).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Organisation).WithMany(o => o.OrganisationUser).HasForeignKey(e => e.OrgId);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organisations> Organisations { get; set; }
        public DbSet<OrganisationUser> OrganisationUsers { get; set; }
    }
}
