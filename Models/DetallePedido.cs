using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace madame_moka.Models
{
    [Table("Detalle_Pedidos")]
    public partial class DetallePedido
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }
        [Column("id_pedido")]
        public int? IdPedido { get; set; }
        [Column("id_producto")]
        public int? IdProducto { get; set; }
        [Column("nombre_producto")]
        [StringLength(100)]
        public string NombreProducto { get; set; } = null!;
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Column("subtotal", TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        [ForeignKey(nameof(IdPedido))]
        [InverseProperty(nameof(Pedido.DetallePedidos))]
        public virtual Pedido? IdPedidoNavigation { get; set; }
        [ForeignKey(nameof(IdProducto))]
        [InverseProperty(nameof(Producto.DetallePedidos))]
        public virtual Producto? IdProductoNavigation { get; set; }
    }
}
