using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Meshkah.Models;

public partial class MeshkahContext : DbContext
{
    public MeshkahContext()
    {
    }

    public MeshkahContext(DbContextOptions<MeshkahContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupType> GroupTypes { get; set; }

    public virtual DbSet<MoneyMovement> MoneyMovements { get; set; }

    public virtual DbSet<MoneyTransaction> MoneyTransactions { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<PointsTransaction> PointsTransactions { get; set; }

    public virtual DbSet<PonitType> PonitTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroupMapping> UserGroupMappings { get; set; }

    public virtual DbSet<UserRoleMapping> UserRoleMappings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=178.62.200.233;Database=Meshkah;Username=doadmin;Password=m0sAibah.s");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Group_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Group_Id_seq\"'::regclass)");

            entity.HasOne(d => d.Type).WithMany(p => p.Groups)
                .HasForeignKey(d => d.GroupTypeId)
                .HasConstraintName("group_groupType");
        });

        modelBuilder.Entity<GroupType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("GroupType_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"GroupType_Id_seq\"'::regclass)");
        });

        modelBuilder.Entity<MoneyMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MoneyMovement_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"MoneyMovement_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.MoneyTransaction).WithMany(p => p.MoneyMovements)
                .HasForeignKey(d => d.MoneyTransactionId)
                .HasConstraintName("MoneyTransaction_MoneyMovement_fk");

            entity.HasOne(d => d.Point).WithMany(p => p.MoneyMovements)
                .HasForeignKey(d => d.PointId)
                .HasConstraintName("Point_MoneyMovement_fk");

            entity.HasOne(d => d.User).WithMany(p => p.MoneyMovements)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_MoneyMovement_fk");
        });

        modelBuilder.Entity<MoneyTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MoneyTransaction_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"MoneyTransaction_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.FromUser).WithMany(p => p.MoneyTransactionFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FromUserId_User_fk");

            entity.HasOne(d => d.ToUser).WithMany(p => p.MoneyTransactionToUsers)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ToUserId_User_fk");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Points_pkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Points)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("points_pointTypes_fk");
        });

        modelBuilder.Entity<PointsTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PointsTransaction_pkey");

            entity.ToTable("PointsTransaction");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Point).WithMany(p => p.PointsTransactions)
                .HasForeignKey(d => d.PointId)
                .HasConstraintName("pointTransaction_point_fk");

            entity.HasOne(d => d.User).WithMany(p => p.PointsTransactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("pointTransaction_user_fk");
        });

        modelBuilder.Entity<PonitType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PonitTypes_pkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Role_Id_seq\"'::regclass)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"User_Id_seq\"'::regclass)");
        });

        modelBuilder.Entity<UserGroupMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserGroupMapping_pkey");

            entity.ToTable("UserGroupMapping");

            entity.HasOne(d => d.Group).WithMany(p => p.UserGroupMappings)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("group_mapping_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserGroupMappings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_mapping_fk");
        });

        modelBuilder.Entity<UserRoleMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserRoleMapping_pkey");

            entity.ToTable("UserRoleMapping");

            entity.HasIndex(e => e.RoleId, "fki_role_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoleMappings)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("role_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoleMappings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_role_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
