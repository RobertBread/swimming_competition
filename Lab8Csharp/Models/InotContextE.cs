using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lab8Csharp.Models;

public partial class InotContextE : DbContext
{
    public InotContextE()
    {
    }

    public InotContextE(DbContextOptions<InotContextE> options)
        : base(options)
    {
    }

    public virtual DbSet<InscriereE> Inscrieres { get; set; }

    public virtual DbSet<ParticipantE> Participants { get; set; }

    public virtual DbSet<ProbaE> Probas { get; set; }

    public virtual DbSet<UserE> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=inot.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InscriereE>(entity =>
        {
            entity.ToTable("Inscriere");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.ProbaId).HasColumnName("proba_id");

            entity.HasOne(d => d.Participant).WithMany(p => p.Inscrieres).HasForeignKey(d => d.ParticipantId);

            entity.HasOne(d => d.Proba).WithMany(p => p.Inscrieres).HasForeignKey(d => d.ProbaId);
        });

        modelBuilder.Entity<ParticipantE>(entity =>
        {
            entity.ToTable("Participant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Name)
                .HasColumnType("varchar(50)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProbaE>(entity =>
        {
            entity.ToTable("Proba");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Distanta)
                .HasColumnType("varchar(50)")
                .HasColumnName("distanta");
            entity.Property(e => e.Stil)
                .HasColumnType("varchar(50)")
                .HasColumnName("stil");
        });

        modelBuilder.Entity<UserE>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "IX_User_username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HashedPassword)
                .HasColumnType("varchar(100)")
                .HasColumnName("hashedPassword");
            entity.Property(e => e.Username)
                .HasColumnType("varchar(50)")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
