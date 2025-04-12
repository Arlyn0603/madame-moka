using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using madame_moka.Models;
using System.Collections.Generic;

namespace madame_moka.Controllers
{
	public class I_eventosController : Controller
	{
		private readonly IConfiguration _configuration;

		public I_eventosController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IActionResult I_eventos()
		{
			List<Evento> eventos = new List<Evento>();

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				string query = "SELECT id_evento, tipo, cantidad, fecha, hora FROM Eventos";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							eventos.Add(new Evento
							{
								IdEvento = reader.GetInt32(0),
								Tipo = reader.GetString(1),
								Cantidad = reader.GetInt32(2),
								Fecha = reader.GetDateTime(3),
								Hora = reader.GetTimeSpan(4)
							});
						}
					}
				}
			}

			return View("i_eventos", eventos); // Pasamos los eventos a la vista
		}
	}
}
