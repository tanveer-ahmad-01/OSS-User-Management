using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.Email, e.ProjectId });
        });

        // Role configurations
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.ProjectId }).IsUnique();
        });

        // Module configurations
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasIndex(e => new { e.Code, e.ProjectId }).IsUnique();
            entity.HasOne(e => e.ParentModule)
                  .WithMany(e => e.SubModules)
                  .HasForeignKey(e => e.ParentModuleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Feature configurations
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasIndex(e => new { e.Code, e.ModuleId }).IsUnique();
        });

        // UserRole configurations
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        // RolePermission configurations
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
        });

        // AuditLog configurations
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.ProjectId);
        });

        // Session configurations
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
        });

        // RefreshToken configurations
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
        });
    }
}

