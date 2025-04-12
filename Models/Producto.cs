using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace madame_moka.Models
{
    public partial class Producto
    {
        public Producto()
        {
            DetallePedidos = new HashSet<DetallePedido>();
        }

        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        [Column("precio", TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; }
        [Column("tipo")]
        [StringLength(50)]
        public string Tipo { get; set; } = null!;
        [Column("imagen_url")]
        [StringLength(255)]
        public string ImagenUrl { get; set; } = null!;

        [InverseProperty(nameof(DetallePedido.IdProductoNavigation))]
        public virtual ICollection<DetallePedido> DetallePedidos { get; set; }
    }
}
