using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using madame_moka.Models;
using madame_moka.Helpers;
using System.Net.Mail;
using System.Net;

namespace madame_moka.Controllers
{
	
	public class PasswordController : Controller
	{
		private readonly IConfiguration _configuration;

		public PasswordController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult Password()
		{
			return View();
		}
		// ✅ 1. Verificar correo y obtener preguntas
		[HttpPost]
		public IActionResult VerificarCorreo([FromBody] dynamic data)
		{
			try
			{
				string correo = data.GetProperty("correo").GetString();

				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					string query = @"SELECT pregunta1, pregunta2, pregunta3 
                                   FROM Usuarios 
                                   WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						conn.Open();

						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								return Json(new
								{
									success = true,
									pregunta1 = reader["pregunta1"].ToString(),
									pregunta2 = reader["pregunta2"].ToString(),
									pregunta3 = reader["pregunta3"].ToString()
								});
							}
						}
					}
				}
				return Json(new { success = false, message = "⚠️ Correo no encontrado." });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error en VerificarCorreo: {ex.Message}");
				return StatusCode(500, new { success = false, message = "Error interno del servidor." });
			}
		}

		// ✅ 2. Validar respuestas y enviar enlace
		[HttpPost]
		public IActionResult ValidarRespuestas([FromBody] dynamic data)
		{
			try
			{
				string correo = data.GetProperty("correo").GetString();
				string respuesta1 = data.GetProperty("respuesta1").GetString();
				string respuesta2 = data.GetProperty("respuesta2").GetString();
				string respuesta3 = data.GetProperty("respuesta3").GetString();


				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();

					// Obtener respuestas guardadas
					string query = @"SELECT respuesta1, respuesta2, respuesta3 
                                   FROM Usuarios 
                                   WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (!reader.Read())
							{
								return Json(new { success = false, message = "❌ Correo no encontrado." });
							}

							// Desencriptar y comparar
							string respuesta1BD = EncryptionHelper.Decrypt(reader["respuesta1"].ToString());
							string respuesta2BD = EncryptionHelper.Decrypt(reader["respuesta2"].ToString());
							string respuesta3BD = EncryptionHelper.Decrypt(reader["respuesta3"].ToString());

							if (respuesta1BD.Equals(respuesta1, StringComparison.OrdinalIgnoreCase) &&
								respuesta2BD.Equals(respuesta2, StringComparison.OrdinalIgnoreCase) &&
								respuesta3BD.Equals(respuesta3, StringComparison.OrdinalIgnoreCase))
							{
								// ✅ Enviar correo con enlace
								return EnviarEnlaceRecuperacion(correo);
							}

							return Json(new { success = false, message = "❌ Respuestas incorrectas." });
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error en ValidarRespuestas: {ex.Message}");
				return Json(new { success = false, message = "Error interno del servidor." });
			}
		}

		// ✅ 3. Enviar correo con enlace de recuperación
		private JsonResult EnviarEnlaceRecuperacion(string correo)
		{
			try
			{
				// Generar token y tiempo de expiración
				string token = Guid.NewGuid().ToString();
				DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

				

				// Guardar en base de datos en la tabla Usuarios
				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"UPDATE Usuarios 
                     SET Tokeen = @Token, expiracion_token = @Expiracion 
                     WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						cmd.Parameters.AddWithValue("@Token", token);
						cmd.Parameters.AddWithValue("@Expiracion", expiracion);
						cmd.ExecuteNonQuery();
					}
				}


				// Generar enlace
				string enlace = $"{Request.Scheme}://{Request.Host}/CambiarContra?token={token}";

				// Configurar correo
				MailMessage mensaje = new MailMessage
				{
					From = new MailAddress("arlinmadriz5@gmail.com"),
					Subject = "Restablecer Contraseña - Madame Moka",
					Body = $@"
                        <h2>Restablecer Contraseña</h2>
                        <p>Haz clic en el siguiente enlace para cambiar tu contraseña:</p>
                        <p><a href='{enlace}'>{enlace}</a></p>
                        <p><em>Este enlace expirará en 15 minutos</em></p>",
					IsBodyHtml = true
				};
				mensaje.To.Add(correo);

				// Enviar usando Gmail
				using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
				{
					smtp.Credentials = new NetworkCredential("arlinmadriz5@gmail.com", "cbogeqxdlbbmjrhk");
					smtp.EnableSsl = true;
					smtp.Send(mensaje);
				}

				return Json(new { success = true, message = "✅ Correo enviado." });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "⚠️ Error: " + ex.Message });
			}
		}
		[HttpGet("CambiarContra")]
		public IActionResult CambiarContra(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				ViewBag.Error = "Token no proporcionado en la URL.";
				return View();
			}

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				conn.Open();
				string query = @"SELECT correo, expiracion_token FROM Usuarios WHERE Tokeen = @Token";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Token", token);

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							DateTime expiracion = reader.GetDateTime(1);
							if (DateTime.UtcNow > expiracion)
							{
								ViewBag.Error = "Token expirado";
								return View();
							}

							HttpContext.Session.SetString("CorreoRecuperacion", reader.GetString(0));
							return View();
						}
					}
				}
			}

			ViewBag.Error = "Token inválido";
			return View();
		}

		[HttpPost("ActualizarContrasena")]
		public JsonResult ActualizarContrasena([FromBody] Dictionary<string, string> data)
		{
			try
			{
				if (!data.ContainsKey("nuevaContrasena"))
				{
					return Json(new { success = false, message = "❌ Falta la nueva contraseña" });
				}

				string nuevaContrasena = data["nuevaContrasena"];
				string correo = HttpContext.Session.GetString("CorreoRecuperacion");

				// Validar sesión
				if (string.IsNullOrEmpty(correo))
				{
					return Json(new { success = false, message = "Sesión expirada" });
				}

				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"UPDATE Usuarios 
                            SET contrasena = @Contrasena, Tokeen = NULL, expiracion_token = NULL
                            WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Contrasena", EncryptionHelper.Encrypt(nuevaContrasena));
						cmd.Parameters.AddWithValue("@Correo", correo);
						cmd.ExecuteNonQuery();
					}
				}

				return Json(new { success = true, message = "✅ Contraseña actualizada correctamente" });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error en ActualizarContrasena: {ex.Message}");
				return Json(new { success = false, message = "Error interno del servidor." });
			}
		}

	}
}
