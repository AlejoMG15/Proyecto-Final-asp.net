﻿// <auto-generated />
using System;
using Cyber360.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cyber360.Migrations
{
    [DbContext(typeof(ProyectoCyberContext))]
    [Migration("20250409133224_arreglos")]
    partial class arreglos
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cyber360.Models.CatProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id")
                        .HasName("PK__CatProdu__3214EC073C69D63E");

                    b.ToTable("CatProduct", (string)null);
                });

            modelBuilder.Entity("Cyber360.Models.CatServi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id")
                        .HasName("PK__CatServi__3214EC07DA984566");

                    b.ToTable("CatServi", (string)null);
                });

            modelBuilder.Entity("Cyber360.Models.Cliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<int?>("Documento")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<int?>("Telefono")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__Clientes__3214EC07C28D00E4");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("Cyber360.Models.Compra", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Fecha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("ProveedorId")
                        .HasColumnType("int");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id")
                        .HasName("PK__Compras__3214EC07AAFF082A");

                    b.HasIndex("ProveedorId");

                    b.ToTable("Compras");
                });

            modelBuilder.Entity("Cyber360.Models.Producto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id")
                        .HasName("PK__Producto__3214EC07FA3A73FF");

                    b.ToTable("Producto", (string)null);
                });

            modelBuilder.Entity("Cyber360.Models.Proveedore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Correo")
                        .HasMaxLength(400)
                        .IsUnicode(false)
                        .HasColumnType("varchar(400)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("NombredeEmpresa")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__Proveedo__3214EC077C551879");

                    b.ToTable("Proveedores");
                });

            modelBuilder.Entity("Cyber360.Models.Servicio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Detalles")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("Id")
                        .HasName("PK__Servicio__3214EC07613C87AA");

                    b.ToTable("Servicio", (string)null);
                });

            modelBuilder.Entity("Cyber360.Models.Venta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<int?>("ClienteId")
                        .HasColumnType("int");

                    b.Property<string>("Estado")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Fecha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("FechaAnulacion")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ProductoId")
                        .HasColumnType("int");

                    b.Property<int?>("ServicioId")
                        .HasColumnType("int");

                    b.Property<decimal?>("valor")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id")
                        .HasName("PK__Ventas__3214EC07C41E0AC0");

                    b.HasIndex("ClienteId");

                    b.HasIndex("ProductoId");

                    b.HasIndex("ServicioId");

                    b.ToTable("Ventas");
                });

            modelBuilder.Entity("Cyber360.Models.Compra", b =>
                {
                    b.HasOne("Cyber360.Models.Proveedore", "Proveedor")
                        .WithMany("Compras")
                        .HasForeignKey("ProveedorId")
                        .IsRequired()
                        .HasConstraintName("FK__Compras__Proveed__5CD6CB2B");

                    b.Navigation("Proveedor");
                });

            modelBuilder.Entity("Cyber360.Models.Venta", b =>
                {
                    b.HasOne("Cyber360.Models.Cliente", "Cliente")
                        .WithMany("Venta")
                        .HasForeignKey("ClienteId")
                        .HasConstraintName("FK__Ventas__ClienteI__5812160E");

                    b.HasOne("Cyber360.Models.Producto", "Producto")
                        .WithMany("Venta")
                        .HasForeignKey("ProductoId")
                        .HasConstraintName("FK__Ventas__Producto__571DF1D5");

                    b.HasOne("Cyber360.Models.Servicio", "Servicio")
                        .WithMany("Venta")
                        .HasForeignKey("ServicioId")
                        .HasConstraintName("FK__Ventas__Servicio__59063A47");

                    b.Navigation("Cliente");

                    b.Navigation("Producto");

                    b.Navigation("Servicio");
                });

            modelBuilder.Entity("Cyber360.Models.Cliente", b =>
                {
                    b.Navigation("Venta");
                });

            modelBuilder.Entity("Cyber360.Models.Producto", b =>
                {
                    b.Navigation("Venta");
                });

            modelBuilder.Entity("Cyber360.Models.Proveedore", b =>
                {
                    b.Navigation("Compras");
                });

            modelBuilder.Entity("Cyber360.Models.Servicio", b =>
                {
                    b.Navigation("Venta");
                });
#pragma warning restore 612, 618
        }
    }
}
