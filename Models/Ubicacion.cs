using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("Ubicaciones")]
public class Ubicacion
{
	[Key]
	[Column("id_ubicacion")]
	public int Id { get; set; }

	[Required]
	[StringLength(100)]
	[Column("nombre")]
	public string Nombre { get; set; }

	[Required]
	[StringLength(20)]
	[Column("tipo")]
	public string Tipo { get; set; } // "Pais", "Provincia", "Canton", "Distrito"

	[Column("id_padre")]
	public int? IdPadre { get; set; }

	[ForeignKey("IdPadre")]
	public virtual Ubicacion Padre { get; set; }

	public virtual ICollection<Ubicacion> Hijos { get; set; } = new List<Ubicacion>();
}