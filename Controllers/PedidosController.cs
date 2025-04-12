using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using madame_moka.Models;
using System.Collections.Generic;

namespace madame_moka.Controllers
{
	public class PickupController : Controller
	{
		private readonly IConfiguration _configuration;

		public PickupController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult Pedidos()
		{
			List<Producto> productos = new List<Producto>();

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				string query = "SELECT id_producto, nombre, descripcion, precio, imagen_url FROM Productos";
				SqlCommand cmd = new SqlCommand(query, conn);
				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						productos.Add(new Producto
						{
							IdProducto = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Descripcion = reader.GetString(2),
							Precio = reader.GetDecimal(3),
							ImagenUrl = reader.GetString(4)
						});
					}
				}
			}


			// 🔹 Pasamos la lista de productos a la vista
			return View("Pedidos", productos);
		}
	}
}
