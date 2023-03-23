using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GestionProductos.Persistence {
	public partial class AplicacionDbContext : DbContext {
		public AplicacionDbContext() {
		}

		public AplicacionDbContext(DbContextOptions<AplicacionDbContext> options)
				: base(options) {
		}

		public virtual DbSet<Producto> Productos { get; set; } = null!;
		public virtual DbSet<Reserva> Reservas { get; set; } = null!;
		public virtual DbSet<Role> Roles { get; set; } = null!;
		public virtual DbSet<User> Users { get; set; } = null!;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			if(!optionsBuilder.IsConfigured) {
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Producto>(entity => {
				entity.HasKey(e => e.IdProducto);

				entity.ToTable("Producto");

				entity.Property(e => e.Barrio)
						.HasMaxLength(50)
						.IsUnicode(false);

				entity.Property(e => e.Codigo)
						.HasMaxLength(10)
						.IsUnicode(false);

				entity.Property(e => e.Imagen)
						.HasMaxLength(100)
						.IsUnicode(false);

				entity.Property(e => e.Precio).HasColumnType("decimal(18, 0)");

				entity.HasMany(d => d.IdReservas)
						.WithMany(p => p.IdProductos)
						.UsingEntity<Dictionary<string, object>>(
								"ReservaProducto",
								l => l.HasOne<Reserva>().WithMany().HasForeignKey("IdReserva").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ReservaProducto_Reserva"),
								r => r.HasOne<Producto>().WithMany().HasForeignKey("IdProducto").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ReservaProducto_Producto"),
								j => {
									j.HasKey("IdProducto", "IdReserva");

									j.ToTable("ReservaProducto");
								});
			});

			modelBuilder.Entity<Reserva>(entity => {
				entity.HasKey(e => e.IdReserva);

				entity.ToTable("Reserva");

				entity.Property(e => e.Cliente)
						.HasMaxLength(100)
						.IsUnicode(false);

				entity.HasOne(d => d.IdVendedorNavigation)
						.WithMany(p => p.Reservas)
						.HasForeignKey(d => d.IdVendedor)
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_Reserva_User");
			});

			modelBuilder.Entity<Role>(entity => {
				entity.ToTable("Role");

				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).HasMaxLength(50);

				entity.HasMany(d => d.Users)
						.WithMany(p => p.Roles)
						.UsingEntity<Dictionary<string, object>>(
								"RoleUser",
								l => l.HasOne<User>().WithMany().HasForeignKey("UsersId"),
								r => r.HasOne<Role>().WithMany().HasForeignKey("RolesId"),
								j => {
									j.HasKey("RolesId", "UsersId");

									j.ToTable("RoleUser");

									j.HasIndex(new[] { "UsersId" }, "IX_RoleUser_UsersId");
								});
			});

			modelBuilder.Entity<User>(entity => {
				entity.ToTable("User");

				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).HasMaxLength(50);

				entity.Property(e => e.PasswordHash).HasMaxLength(100);

				entity.Property(e => e.Username).HasMaxLength(50);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
