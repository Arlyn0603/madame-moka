using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace madame_moka.Models
{
	[Index(nameof(Correo), Name = "IX_Usuarios_Correo_Unico", IsUnique = true)]
	public class Usuario : IValidatableObject
	{
		public Usuario()
		{
			Eventos = new HashSet<Evento>();
			Pedidos = new HashSet<Pedido>();
			Reservas = new HashSet<Reserva>();
		}

		[Key]
		[Column("id_usuario")]
		public int IdUsuario { get; set; }

		[Required]
		[Column("nombre")]
		[StringLength(100)]
		public string Nombre { get; set; } = null!;

		[Column("telefono")]
		[StringLength(15)]
		public string? Telefono { get; set; }

		[Required]
		[EmailAddress]
		[Column("correo")]
		[StringLength(100)]
		public string Correo { get; set; } = null!;

		[Range(1, 120)]
		[Column("edad")]
		public int? Edad { get; set; }

		[Required]
		[Column("contrasena")]
		[StringLength(255)]
		public string Contrasena { get; set; } = null!;

		[Column("token_verificacion")]
		[StringLength(6)]
		public string? Token_verificacion { get; set; }

		[Required]
		[Column("respuesta1")]
		public string Respuesta1 { get; set; }

		[Required]
		[Column("respuesta2")]
		public string Respuesta2 { get; set; }

		[Required]
		[Column("respuesta3")]
		public string Respuesta3 { get; set; }

		

		[InverseProperty(nameof(Evento.IdUsuarioNavigation))]
		public virtual ICollection<Evento> Eventos { get; set; }

		[InverseProperty(nameof(Pedido.IdUsuarioNavigation))]
		public virtual ICollection<Pedido> Pedidos { get; set; }

		[InverseProperty(nameof(Reserva.IdUsuarioNavigation))]
		public virtual ICollection<Reserva> Reservas { get; set; }

		// ✅ Relación con Ubicaciones (País, Provincia, Cantón y Distrito)
		[Column("direccion")]
		[StringLength(100)]
		public string? Direccion { get; set; } // Nueva propiedad para la dirección compuesta



		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!EsContrasenaValida(Contrasena))
			{
				yield return new ValidationResult("La contraseña debe tener al menos una mayúscula, un número, un símbolo y tener entre 7 y 21 caracteres.", new[] { nameof(Contrasena) });
			}
		}

		private bool EsContrasenaValida(string contrasena)
		{
			if (string.IsNullOrEmpty(contrasena)) return false;
			if (contrasena.Length < 7 || contrasena.Length > 21) return false;

			// Expresión regular: al menos una mayúscula, un número y un símbolo
			return Regex.IsMatch(contrasena, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{7,21}$");
		}
	}
}
