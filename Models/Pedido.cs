using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace madame_moka.Models
{
    public partial class Pedido
    {
        public Pedido()
        {
            DetallePedidos = new HashSet<DetallePedido>();
        }

        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }
        [Column("id_usuario")]
        public int? IdUsuario { get; set; }
        [Column("fecha_pedido", TypeName = "datetime")]
        public DateTime? FechaPedido { get; set; }
        [Column("metodo_pago")]
        [StringLength(50)]
        public string MetodoPago { get; set; } = null!;
        [Column("total", TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        [InverseProperty(nameof(Usuario.Pedidos))]
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        [InverseProperty(nameof(DetallePedido.IdPedidoNavigation))]
        public virtual ICollection<DetallePedido> DetallePedidos { get; set; }
    }
}
