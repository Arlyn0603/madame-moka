using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using madame_moka.Models;
using System.Collections.Generic;

namespace madame_moka.Controllers
{
	public class I_pedidosController : Controller
	{
		private readonly IConfiguration _configuration;

		public I_pedidosController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IActionResult i_pedidos()
		{
			List<DetallePedido> pedidos = new List<DetallePedido>();

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				string query = "SELECT nombre_producto, cantidad, subtotal FROM Detalle_Pedidos";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					conn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							pedidos.Add(new DetallePedido
							{
								NombreProducto = reader.GetString(0),
								Cantidad = reader.GetInt32(1),
								Subtotal = reader.GetDecimal(2)
							});
						}
					}
				}
			}

			return View("i_pedidos", pedidos); // Enviamos los pedidos a la vista
		}
	}
}
