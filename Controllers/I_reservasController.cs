using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using madame_moka.Models;
using System.Collections.Generic;

namespace madame_moka.Controllers
{
	public class I_ReservasController : Controller
	{
		private readonly IConfiguration _configuration;

		public I_ReservasController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IActionResult I_reservas()
		{
			List<Reserva> reservas = new List<Reserva>();

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				string query = "SELECT id_reserva, cantidad, fecha, hora, tipo FROM Reservas"; // ❌ Se eliminó id_usuario
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							reservas.Add(new Reserva
							{
								IdReserva = reader.GetInt32(0),
								Cantidad = reader.GetInt32(1),
								Fecha = reader.GetDateTime(2),
								Hora = reader.GetTimeSpan(3),
								Tipo = reader.GetString(4)
							});
						}
					}
				}
			}

			return View("i_reservas", reservas); // 🔹 Pasamos las reservas a la vista
		}
	}
}
