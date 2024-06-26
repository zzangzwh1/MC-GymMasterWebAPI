﻿using System;
using System.Collections.Generic;
using MC_GymMasterWebAPI.Model;
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

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberConnection> MemberConnections { get; set; }

    public virtual DbSet<ShareBoard> ShareBoards { get; set; }

    public virtual DbSet<WorkoutSet> WorkoutSets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localDB)\\MSSQLLocalDB;Database=GymMaster;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chat__A9FBE7C6BC8A14C9");

            entity.ToTable("Chat");

            entity.HasOne(d => d.Member).WithMany(p => p.Chats)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chat_Member");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B187936CA49");

            entity.ToTable("Member");

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
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
            entity.HasKey(e => e.MemberConnectionId).HasName("PK__MemberCo__807A9DF605D3176D");

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
            entity.HasKey(e => e.ShareBoardId).HasName("PK__ShareBoa__C0C706601EDDD30A");

            entity.ToTable("ShareBoard");

            entity.Property(e => e.ShareBoardId).ValueGeneratedNever();

            entity.HasOne(d => d.Member).WithMany(p => p.ShareBoards)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShareBoard_Member");
        });

        modelBuilder.Entity<WorkoutSet>(entity =>
        {
            entity.HasKey(e => e.WorkoutSetId).HasName("PK__WorkoutS__A55C57242A79CCA4");

            entity.ToTable("WorkoutSet");

            entity.Property(e => e.Part).HasMaxLength(20);
            entity.Property(e => e.SetDescription).HasMaxLength(100);

            entity.HasOne(d => d.Member).WithMany(p => p.WorkoutSets)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkoutSet_Member");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
