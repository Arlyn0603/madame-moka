using madame_moka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace madame_moka.Controllers
{
	public class CarritoController : Controller
	{
		private readonly IConfiguration _configuration;

		public CarritoController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		private int GetCurrentUserId()
		{
			if (User.Identity.IsAuthenticated)
			{
				var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
				return int.Parse(claim.Value);
			}
			return 0; // Manejar usuarios no autenticados según tu lógica
		}

		[HttpGet]
		public IActionResult Carrito()
		{
			return View("Carrito");
		}

		[HttpPost]
		public IActionResult SolicitarPedido([FromBody] PedidoRequest request)
		{
			if (request == null || request.Detalles == null || request.Detalles.Count == 0)
			{
				return BadRequest(new { success = false, message = "El carrito está vacío." });
			}

			try
			{
				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					using (SqlCommand cmd = new SqlCommand("InsertarPedido", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@id_usuario", DBNull.Value); // No enviar ID de usuario
						cmd.Parameters.AddWithValue("@metodo_pago", request.MetodoPago);
						cmd.Parameters.AddWithValue("@total", request.Total);

						// 🔹 Serializa los detalles a JSON correctamente
						string detallesJson = JsonSerializer.Serialize(request.Detalles);
						cmd.Parameters.AddWithValue("@detalles", detallesJson);

						conn.Open();
						int idPedido = Convert.ToInt32(cmd.ExecuteScalar());
						conn.Close();

						return Ok(new { success = true, idPedido });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}

		}
	}

	public class PedidoRequest
	{
		public string MetodoPago { get; set; }
		public decimal Total { get; set; }
		public List<DetallePedidoRequest> Detalles { get; set; }
	}

	public class DetallePedidoRequest
	{
		public int IdProducto { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public decimal Total { get; set; }
	}
}
