using System;
using System.Collections.Generic;
using MC_GymMasterWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MC_GymMasterWebAPI.Data;

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

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ImageLike> ImageLikes { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberConnection> MemberConnections { get; set; }

    public virtual DbSet<ShareBoard> ShareBoards { get; set; }

    public virtual DbSet<WorkoutSet> WorkoutSets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:GymConnection");

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

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chat__A9FBE7C6D5AF42AB");

            entity.ToTable("Chat");

            entity.HasOne(d => d.Member).WithMany(p => p.Chats)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chat_Member");
        });

        modelBuilder.Entity<ImageLike>(entity =>
        {
            entity.HasKey(e => e.LikeImageId).HasName("PK__ImageLik__BD64871AC63DDBC0");

            entity.ToTable("ImageLike");

            entity.Property(e => e.Like).HasColumnName("ImageLike");

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

        modelBuilder.Entity<MemberConnection>(entity =>
        {
            entity.HasKey(e => e.MemberConnectionId).HasName("PK__MemberCo__807A9DF68E0F28DD");

            entity.ToTable("MemberConnection");

            entity.Property(e => e.Followers).HasMaxLength(100);
            entity.Property(e => e.Followings).HasMaxLength(100);

            entity.HasOne(d => d.Member).WithMany(p => p.MemberConnections)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberConnection_Member");
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
