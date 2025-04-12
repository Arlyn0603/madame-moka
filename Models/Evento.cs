using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace madame_moka.Models
{
    public partial class Evento
    {
        [Key]
        [Column("id_evento")]
        public int IdEvento { get; set; }
        [Column("id_usuario")]
        public int? IdUsuario { get; set; }
        [Column("tipo")]
        [StringLength(50)]
        public string Tipo { get; set; } = null!;
        [Column("descripcion")]
        public string? Descripcion { get; set; }
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Column("fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }
        [Column("hora")]
        public TimeSpan Hora { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        [InverseProperty(nameof(Usuario.Eventos))]
        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}
