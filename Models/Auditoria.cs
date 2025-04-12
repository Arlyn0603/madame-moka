using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace madame_moka.Models
{
	public class Auditoria
	{
		[Key]
		[Column("IdAuditoria")]
		public int IdAuditoria { get; set; }

		[Required]
		[Column("IdUsuario")]
		public int IdUsuario { get; set; }

		[Required]
		[Column("Movimiento")]
		public int Movimiento { get; set; }

		[Required]
		[Column("Descripcion")]
		[StringLength(255)]
		public string Descripcion { get; set; } = null!;

		[Column("FechaHora")]
		public DateTime FechaHora { get; set; } = DateTime.Now;

		[ForeignKey("IdUsuario")]
		public virtual Usuario Usuario { get; set; } = null!;
	}
}
