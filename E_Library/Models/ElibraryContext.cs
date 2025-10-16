using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace E_Library.Models;


public partial class ElibraryContext : DbContext
{
    public ElibraryContext()
    {
    }

    public ElibraryContext(DbContextOptions<ElibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Checkout> Checkouts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<LibraryCard> LibraryCards { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var builder = new ConfigurationBuilder(); //Microsoft.Extensions...
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
       // => optionsBuilder.UseSqlServer("Server=DESKTOP-7KQPR9J;uid=sa;password=123456789;database=ELibrary;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AuditLog__5E5499A867BAD6E3");

            entity.ToTable("AuditLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.TableName).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLog__Employ__7C4F7684");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Author__70DAFC144F680503");

            entity.ToTable("Author");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.AuthorName).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C227116C139B");

            entity.ToTable("Book");

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Book__AuthorID__68487DD7");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Book__CategoryID__6754599E");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Book__PublisherI__66603565");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BF79906AA");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Checkout>(entity =>
        {
            entity.HasKey(e => e.CheckoutId).HasName("PK__Checkout__E07EF51C76422F72");

            entity.ToTable("Checkout");

            entity.Property(e => e.CheckoutId).HasColumnName("CheckoutID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Book).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Checkout__BookID__6C190EBB");

            entity.HasOne(d => d.Reader).WithMany(p => p.Checkouts)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Checkout__Reader__6D0D32F4");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF103E8C2E1");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D10534EE23C75E").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__RoleID__5441852A");
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.FineId).HasName("PK__Fine__9D4A9BCC21A6BA5D");

            entity.ToTable("Fine");

            entity.Property(e => e.FineId).HasColumnName("FineID");
            entity.Property(e => e.CheckoutId).HasColumnName("CheckoutID");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FineDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FineType).HasMaxLength(100);
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Checkout).WithMany(p => p.Fines)
                .HasForeignKey(d => d.CheckoutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Fine__CheckoutID__71D1E811");

            entity.HasOne(d => d.Reader).WithMany(p => p.Fines)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Fine__ReaderID__72C60C4A");
        });

        modelBuilder.Entity<LibraryCard>(entity =>
        {
            entity.HasKey(e => e.CardId).HasName("PK__LibraryC__55FECD8E50BD9E46");

            entity.ToTable("LibraryCard");

            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Reader).WithMany(p => p.LibraryCards)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LibraryCa__Reade__5BE2A6F2");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F76B18D67");

            entity.ToTable("Permission");

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657E4B774B2CB5");

            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.PublisherName).HasMaxLength(100);
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderId).HasName("PK__Reader__8E67A581C4F6960D");

            entity.ToTable("Reader");

            entity.HasIndex(e => e.Email, "UQ__Reader__A9D105341AE797AF").IsUnique();

            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F04A041F6B6");

            entity.ToTable("Reservation");

            entity.Property(e => e.ReservationId).HasColumnName("ReservationID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.ReaderId).HasColumnName("ReaderID");
            entity.Property(e => e.ReservationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Book).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__BookI__787EE5A0");

            entity.HasOne(d => d.Reader).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Reade__778AC167");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A0C0D28A6");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_Perm__Permi__4E88ABD4"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_Perm__RoleI__4D94879B"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__Role_Per__6400A188D5EFD93B");
                        j.ToTable("Role_Permission");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
