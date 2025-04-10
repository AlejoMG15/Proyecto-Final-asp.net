using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cyber360.Models;

public partial class ProyectoCyberContext : DbContext
{
    public ProyectoCyberContext()
    {
    }

    public ProyectoCyberContext(DbContextOptions<ProyectoCyberContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatProduct> CatProducts { get; set; }

    public virtual DbSet<CatServi> CatServis { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LEJOMG\\SQLEXPRESS;Initial Catalog=ProyectoCyber;integrated security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatProdu__3214EC073C69D63E");

            entity.ToTable("CatProduct");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);

            // Configuración de la relación inversa
            entity.HasMany(c => c.Productos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId);
        });

        modelBuilder.Entity<CatServi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CatServi__3214EC07DA984566");

            entity.ToTable("CatServi");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);

            // Configuración de la relación inversa
            entity.HasMany(c => c.Servicios)
                .WithOne(s => s.Categoria)
                .HasForeignKey(s => s.CategoriaId);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC07C28D00E4");

            entity.Property(e => e.Apellido)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Compras__3214EC07AAFF082A");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Compras)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compras__Proveed__5CD6CB2B");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC07FA3A73FF");

            entity.ToTable("Producto");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            // Configuración de la relación con CatProduct
            entity.HasOne(p => p.Categoria)          // Propiedad de navegación
                .WithMany(c => c.Productos)          // Colección en CatProduct
                .HasForeignKey(p => p.CategoriaId)   // Nombre real de la columna FK
                .OnDelete(DeleteBehavior.ClientSetNull) // Comportamiento al eliminar
                .HasConstraintName("FK_Producto_Categoria"); // Nombre del constraint

            // Si necesitas mapear explícitamente el nombre de la columna:
            entity.Property(e => e.CategoriaId)
                .HasColumnName("CategoriaId") // Asegúrate que coincida con tu BD
                .IsRequired();
        });


        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proveedo__3214EC077C551879");

            entity.Property(e => e.Correo)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombredeEmpresa)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Servicio__3214EC07613C87AA");

            entity.ToTable("Servicio");

            entity.Property(e => e.Detalles)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            // Configuración de la relación con CatServi
            entity.HasOne(s => s.Categoria)          // Propiedad de navegación
                .WithMany(c => c.Servicios)         // Colección en CatServi
                .HasForeignKey(s => s.CategoriaId)  // Nombre real de la columna FK
                .OnDelete(DeleteBehavior.ClientSetNull) // Comportamiento al eliminar
                .HasConstraintName("FK_Servicio");   // Nombre del constraint

            // Si necesitas mapear explícitamente el nombre de la columna:
            entity.Property(e => e.CategoriaId)
                .HasColumnName("CategoriaId") // Asegúrate que coincida con tu BD
                .IsRequired();
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ventas__3214EC07C41E0AC0");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Venta)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Ventas__ClienteI__5812160E");

            entity.HasOne(d => d.Producto).WithMany(p => p.Venta)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__Ventas__Producto__571DF1D5");

            entity.HasOne(d => d.Servicio).WithMany(p => p.Venta)
                .HasForeignKey(d => d.ServicioId)
                .HasConstraintName("FK__Ventas__Servicio__59063A47");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
