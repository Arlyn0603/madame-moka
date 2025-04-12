using madame_moka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace madame_moka.Controllers
{
	public class EventosController : Controller
	{
		private readonly string _connectionString;

		public EventosController(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		// ✅ Método GET para mostrar la vista de eventos correctamente
		[HttpGet]
		public IActionResult Eventos()
		{
			return View("eventos"); // Carga la vista eventos.cshtml
		}

		// ✅ Método POST para guardar el evento
		[HttpPost]
		public IActionResult Eventos(Evento model)
		{
			if (ModelState.IsValid)
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("InsertarEvento", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@id_usuario", model.IdUsuario ?? (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@tipo", model.Tipo);
						cmd.Parameters.AddWithValue("@cantidad", model.Cantidad);
						cmd.Parameters.AddWithValue("@fecha", model.Fecha);
						cmd.Parameters.AddWithValue("@hora", model.Hora);

						conn.Open();
						cmd.ExecuteNonQuery();
					}
				}
				return RedirectToAction("Eventos"); // 🔹 Redirige a la misma vista después de guardar
			}
			return View("eventos", model);
		}
	}
}
