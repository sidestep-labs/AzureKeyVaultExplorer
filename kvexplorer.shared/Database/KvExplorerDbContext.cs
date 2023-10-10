using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace kvexplorer.shared.Database;

public partial class KvExplorerDbContext : DbContext
{
    public KvExplorerDbContext()
    {
    }

    public KvExplorerDbContext(DbContextOptions<KvExplorerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookmarkedItem> BookmarkedItems { get; set; }

    public virtual DbSet<QuickAccess> QuickAccessItems { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlite("");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookmarkedItem>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.Version }, "IX_BookmarkedItems_Name_Version").IsUnique();

            entity.Property(e => e.Type).HasColumnType("INT");
        });

        modelBuilder.Entity<QuickAccess>(entity =>
        {
            entity.ToTable("QuickAccess");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
