using Microsoft.EntityFrameworkCore;
using madame_moka.Models;

namespace madame_moka.Data
{

	public partial class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext()
		{
		}

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}


		// 🔹 DbSet de Ubicaciones agregado
		public virtual DbSet<Ubicacion> Ubicaciones { get; set; } = null!;

		// 🔹 Otros DbSet existentes
		public virtual DbSet<Auditoria> Auditorias { get; set; } = null!;
		public virtual DbSet<DetallePedido> DetallePedidos { get; set; } = null!;
		public virtual DbSet<Evento> Eventos { get; set; } = null!;
		public virtual DbSet<Pedido> Pedidos { get; set; } = null!;
		public virtual DbSet<Producto> Productos { get; set; } = null!;
		public virtual DbSet<Reserva> Reservas { get; set; } = null!;
		public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

	

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// 🔹 Configuración de Ubicaciones
			modelBuilder.Entity<Ubicacion>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Nombre)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.Tipo)
					.IsRequired()
					.HasMaxLength(20);

				entity.HasOne(u => u.Padre)
					.WithMany(u => u.Hijos)
					.HasForeignKey(u => u.IdPadre)
					.OnDelete(DeleteBehavior.NoAction);
			});


			// Configuración de Auditoria (Nuevo)
			modelBuilder.Entity<Auditoria>(entity =>
			{
				entity.HasKey(e => e.IdAuditoria); // Clave primaria

				// Relación con Usuario
				entity.HasOne(a => a.Usuario)
					.WithMany()
					.HasForeignKey(a => a.IdUsuario)
					.HasConstraintName("FK_Auditoria_Usuario");
			});

			// Resto de configuraciones existentes...
			modelBuilder.Entity<DetallePedido>(entity =>
			{
				entity.HasKey(e => e.IdDetalle)
					.HasName("PK__Detalle___4F1332DE3A791678");

				entity.HasOne(d => d.IdPedidoNavigation)
					.WithMany(p => p.DetallePedidos)
					.HasForeignKey(d => d.IdPedido)
					.HasConstraintName("FK__Detalle_P__id_pe__45F365D3");

				entity.HasOne(d => d.IdProductoNavigation)
					.WithMany(p => p.DetallePedidos)
					.HasForeignKey(d => d.IdProducto)
					.HasConstraintName("FK__Detalle_P__id_pr__46E78A0C");
			});

			modelBuilder.Entity<Evento>(entity =>
			{
				entity.HasKey(e => e.IdEvento)
					.HasName("PK__Eventos__AF150CA5D946EC1B");

				entity.HasOne(d => d.IdUsuarioNavigation)
					.WithMany(p => p.Eventos)
					.HasForeignKey(d => d.IdUsuario)
					.HasConstraintName("FK__Eventos__id_usua__3F466844");
			});

			modelBuilder.Entity<Pedido>(entity =>
			{
				entity.HasKey(e => e.IdPedido)
					.HasName("PK__Pedidos__6FF014893916704F");

				entity.Property(e => e.FechaPedido).HasDefaultValueSql("(getdate())");

				entity.HasOne(d => d.IdUsuarioNavigation)
					.WithMany(p => p.Pedidos)
					.HasForeignKey(d => d.IdUsuario)
					.HasConstraintName("FK__Pedidos__id_usua__4316F928");
			});

			modelBuilder.Entity<Producto>(entity =>
			{
				entity.HasKey(e => e.IdProducto)
					.HasName("PK__Producto__FF341C0D941F17A7");
			});

			modelBuilder.Entity<Reserva>(entity =>
			{
				entity.HasKey(e => e.IdReserva)
					.HasName("PK__Reservas__423CBE5D064BD31E");

				entity.HasOne(d => d.IdUsuarioNavigation)
					.WithMany(p => p.Reservas)
					.HasForeignKey(d => d.IdUsuario)
					.HasConstraintName("FK__Reservas__id_usu__3C69FB99");
			});

			modelBuilder.Entity<Usuario>(entity =>
			{
				entity.HasKey(e => e.IdUsuario)
					.HasName("PK__Usuarios__4E3E04AD5B86EB9E");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
