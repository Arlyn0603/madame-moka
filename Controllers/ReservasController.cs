using madame_moka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace madame_moka.Controllers
{
	public class ReservasController : Controller
	{
		private readonly string _connectionString;

		public ReservasController(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		// ✅ Método GET para mostrar la vista de reservas correctamente
		[HttpGet]
		public IActionResult Reservas()
		{
			return View("reservas"); // Carga la vista reservas.cshtml
		}

		// ✅ Método POST para guardar la reserva
		[HttpPost]
		public IActionResult Reservas(Reserva model)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("InsertarReserva", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@id_usuario", model.IdUsuario ?? (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@cantidad", model.Cantidad);
						cmd.Parameters.AddWithValue("@fecha", model.Fecha);
						cmd.Parameters.AddWithValue("@hora", model.Hora);
						cmd.Parameters.AddWithValue("@tipo", model.Tipo);

						conn.Open();
						cmd.ExecuteNonQuery();
					}
				}
				return RedirectToAction("Reservas"); // 🔹 Redirige a la misma vista después de guardar
			}
			return View("reservas", model);
		}
	}
}
