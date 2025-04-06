using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MC_GymMasterWebAPI.Models;

public partial class GymMasterContext : DbContext
{
    public GymMasterContext()
    {
    }

    public GymMasterContext(DbContextOptions<GymMasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BoardComment> BoardComments { get; set; }

    public virtual DbSet<ImageLike> ImageLikes { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<ShareBoard> ShareBoards { get; set; }

    public virtual DbSet<WorkoutSet> WorkoutSets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localDB)\\MSSQLLocalDB;Database=GymMaster;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoardComment>(entity =>
        {
            entity.HasKey(e => e.BoardCommentId).HasName("PK__BoardCom__D2B2E296B02DA4BA");

            entity.ToTable("BoardComment");

            entity.Property(e => e.Comment).HasColumnName("comment");

            entity.HasOne(d => d.ShareBoard).WithMany(p => p.BoardComments)
                .HasForeignKey(d => d.ShareBoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoardComment_ShareBoard");
        });

        modelBuilder.Entity<ImageLike>(entity =>
        {
            entity.HasKey(e => e.LikeImageId).HasName("PK__ImageLik__BD64871AC1811921");

            entity.ToTable("ImageLike");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");

            entity.HasOne(d => d.ShareBoard).WithMany(p => p.ImageLikes)
                .HasForeignKey(d => d.ShareBoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImageLike_ShareBoard");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B18D19BFC9A");

            entity.ToTable("Member");

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Sex)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<ShareBoard>(entity =>
        {
            entity.HasKey(e => e.ShareBoardId).HasName("PK__ShareBoa__C0C70660D413E94B");

            entity.ToTable("ShareBoard");

            entity.HasOne(d => d.Member).WithMany(p => p.ShareBoards)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShareBoard_Member");
        });

        modelBuilder.Entity<WorkoutSet>(entity =>
        {
            entity.HasKey(e => e.WorkoutSetId).HasName("PK__WorkoutS__A55C5724FD7CF770");

            entity.ToTable("WorkoutSet");

            entity.Property(e => e.Part).HasMaxLength(50);
            entity.Property(e => e.SetDescription).HasMaxLength(100);
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Member).WithMany(p => p.WorkoutSets)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutSet_Member");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
