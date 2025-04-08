using System;
using System.Collections.Generic;
using LoginAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginAuthApi.DAL;

public partial class EccomerceDbContext : DbContext
{
    public EccomerceDbContext()
    {
    }

    public EccomerceDbContext(DbContextOptions<EccomerceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-NPGUFTMO\\SQLEXPRESS;Database=EccomerceDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.AutRoleId).HasName("PK__ROLES__6507A2A9E0148451");

            entity.ToTable("ROLES");

            entity.Property(e => e.AutRoleId)
                .ValueGeneratedNever()
                .HasColumnName("autRoleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.AutId).HasName("PK__USERS__41C6DD8BB4A5A9F3");

            entity.ToTable("USERS");

            entity.Property(e => e.AutId).HasColumnName("autID");
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.Username).IsRequired();

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USERS__roleId__2B3F6F97");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
