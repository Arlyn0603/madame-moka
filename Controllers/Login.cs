using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net.Mail;
using System.Net;
using madame_moka.Models;
using madame_moka.Helpers;

namespace madame_moka.Controllers
{
	[Route("[controller]")]
	public class LoginController : Controller
	{
		private readonly IConfiguration _configuration;

		public LoginController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult IrAUsuario()
		{
			return RedirectToAction("Usuario", "usuario"); // 🔹 Redirige a la vista Usuario
		}

		[HttpPost("VerificarCredenciales")]
		public JsonResult VerificarCredenciales([FromBody] dynamic data)
		{
			try
			{
				string correo = data.GetProperty("correo").GetString();
				string contrasenaIngresada = data.GetProperty("contrasena").GetString();

				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"SELECT contrasena FROM Usuarios WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						object resultado = cmd.ExecuteScalar();

						if (resultado == null)
						{
							return Json(new { success = false, message = "❌ Correo no registrado." });
						}

						// 🔹 Desencriptar la contraseña almacenada en la BD
						string contrasenaEnBD = EncryptionHelper.Decrypt(resultado.ToString());

						// 🔹 Comparar contraseñas
						if (!contrasenaEnBD.Equals(contrasenaIngresada))
						{
							return Json(new { success = false, message = "❌ Contraseña incorrecta." });
						}
					}
				}

				// ✅ Si las credenciales son correctas, generar y actualizar el token en la BD
				return EnviarCodigoVerificacion(correo);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ ERROR en VerificarCredenciales: {ex.Message}");
				return Json(new { success = false, message = "⚠️ Error interno en el servidor." });
			}
		}

		private JsonResult EnviarCodigoVerificacion(string correo)
		{
			try
			{
				// 🔹 Generar código aleatorio de 6 dígitos
				Random random = new Random();
				int codigo = random.Next(100000, 999999);
				DateTime tiempoExpiracion = DateTime.UtcNow.AddMinutes(5);

				// 🔹 Guardar en la base de datos el código en `token_verificacion`
				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"UPDATE Usuarios SET token_verificacion = @Token WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						cmd.Parameters.AddWithValue("@Token", codigo.ToString());
						cmd.ExecuteNonQuery();
					}
				}

				Console.WriteLine($"📌 Código generado y guardado en BD: {codigo}");

				// 🔹 Enviar correo con el código
				try
				{
					MailMessage mail = new MailMessage();
					mail.From = new MailAddress("arlinmadriz5@gmail.com");
					mail.To.Add(correo);
					mail.Subject = "Código de Verificación - Madame Moka";
					mail.Body = $"<p>Tu código de verificación es: <strong>{codigo}</strong></p>";
					mail.IsBodyHtml = true;

					SmtpClient smtp = new SmtpClient("smtp.gmail.com")
					{
						Port = 587,
						Credentials = new NetworkCredential("arlinmadriz5@gmail.com", "cbogeqxdlbbmjrhk"),
						EnableSsl = true
					};

					smtp.Send(mail);
				}
				catch (Exception ex)
				{
					return Json(new { success = false, message = "⚠️ Error al enviar el correo: " + ex.Message });
				}

				return Json(new { success = true, message = "✅ Código enviado. Revisa tu correo electrónico." });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ ERROR en EnviarCodigoVerificacion: {ex.Message}");
				return Json(new { success = false, message = "⚠️ Error interno en el servidor." });
			}
		}
		[HttpPost("VerificarCodigo")]
		public JsonResult VerificarCodigo([FromBody] dynamic data)
		{
			try
			{
				string correo = data.GetProperty("correo").GetString();
				string codigoIngresado = data.GetProperty("codigoIngresado").GetString();

				using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
				{
					conn.Open();
					string query = @"SELECT token_verificacion FROM Usuarios WHERE correo = @Correo";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Correo", correo);
						object resultado = cmd.ExecuteScalar();

						if (resultado == null)
						{
							return Json(new { success = false, message = "❌ Correo no encontrado." });
						}

						string codigoGuardado = resultado.ToString();

						if (codigoIngresado == codigoGuardado)
						{
							return Json(new { success = true, message = "✅ Código verificado correctamente." });
						}
						else
						{
							return Json(new { success = false, message = "❌ Código incorrecto." });
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ ERROR en VerificarCodigo: {ex.Message}");
				return Json(new { success = false, message = "⚠️ Error interno en el servidor." });
			}
		}


	}
}