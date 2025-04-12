using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net.Mail;
using System.Net;
using madame_moka.Models;
using madame_moka.Helpers;
[Route("[controller]")]
public class RegistroController : Controller
{
	private readonly IConfiguration _configuration;

	public RegistroController(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[HttpGet]
	public IActionResult Registro()
	{
		return View("Registro");
	}

	[HttpPost("VerificarCodigo")]
	public JsonResult VerificarCodigo([FromBody] dynamic data)
	{
		string codigoIngresado = data.GetProperty("codigoIngresado").GetString();

		string codigoGuardado = HttpContext.Session.GetString("CodigoVerificacion");
		string correoVerificar = HttpContext.Session.GetString("CorreoVerificar");
		string tiempoExpiracionStr = HttpContext.Session.GetString("CodigoExpiracion");

		if (string.IsNullOrEmpty(codigoGuardado) || string.IsNullOrEmpty(correoVerificar) || string.IsNullOrEmpty(tiempoExpiracionStr))
		{
			return Json(new { success = false, message = "⚠️ No se ha enviado ningún código o la sesión ha expirado." });
		}

		// 🔹 Imprimir los valores para depuración
		Console.WriteLine($"🔹 Código ingresado: {codigoIngresado}");
		Console.WriteLine($"🔹 Código guardado en sesión: {codigoGuardado}");
		Console.WriteLine($"🔹 Tiempo de expiración guardado en sesión: {tiempoExpiracionStr}");
		Console.WriteLine($"🔹 Fecha actual en UTC: {DateTime.UtcNow}");

		DateTime tiempoExpiracion;
		bool esFechaValida = DateTime.TryParse(tiempoExpiracionStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out tiempoExpiracion);

		if (!esFechaValida)
		{
			return Json(new { success = false, message = "⚠️ Error al verificar el tiempo de expiración." });
		}

		// 🔹 Comparar con la hora UTC actual
		DateTime ahoraUtc = DateTime.UtcNow;
		Console.WriteLine($"🔹 Comparando ahora ({ahoraUtc}) con expiración ({tiempoExpiracion})");

		if (ahoraUtc > tiempoExpiracion)
		{
			return Json(new { success = false, message = "⏳ Código expirado. Solicita uno nuevo." });
		}

		if (codigoIngresado == codigoGuardado)
		{
			HttpContext.Session.Remove("CodigoVerificacion");
			HttpContext.Session.Remove("CodigoExpiracion");
			return Json(new { success = true, message = "✅ Código verificado correctamente.", correo = correoVerificar });
		}

		return Json(new { success = false, message = "❌ Código incorrecto. Inténtalo de nuevo." });
	}

	[HttpPost("EnviarCodigoVerificacion")]
	public JsonResult EnviarCodigoVerificacion([FromBody] dynamic data)
	{
		try
		{
			string correo = data.GetProperty("correo").GetString();

			// 🔹 Verificar si el correo ya está registrado
			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				conn.Open();
				string query = "SELECT COUNT(*) FROM Usuarios WHERE correo = @Correo";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@Correo", correo);
					int count = Convert.ToInt32(cmd.ExecuteScalar());

					if (count > 0)
					{
						return Json(new { success = false, message = "❌ Este correo ya está registrado. Por favor, ingresa otro." });
					}
				}
			}

			// 🔹 Generar código aleatorio de 6 dígitos
			Random random = new Random();
			int codigo = random.Next(100000, 999999);
			DateTime tiempoExpiracion = DateTime.UtcNow.AddMinutes(2);

			// 🔹 Guardar en sesión
			HttpContext.Session.SetString("CodigoVerificacion", codigo.ToString());
			HttpContext.Session.SetString("CorreoVerificar", correo);
			HttpContext.Session.SetString("CodigoExpiracion", tiempoExpiracion.ToString("o"));

			Console.WriteLine($"📌 Código generado y guardado en sesión: {codigo}");

			// 🔹 Enviar correo (mantenemos esta parte igual)
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
			return Json(new { success = false, message = "⚠️ Error interno en el servidor: " + ex.Message });
		}
	}
	[HttpPost]
	public IActionResult RegistrarUsuario(Usuario model, string Pais, string Provincia, string Canton, string Distrito)
	{
		if (ModelState.IsValid)
		{
			// ✅ Concatenar los IDs de ubicación para formar la dirección compuesta
			model.Direccion = $"{Pais}-{Provincia}-{Canton}-{Distrito}";

			using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
			{
				using (SqlCommand cmd = new SqlCommand("InsertarUsuario", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// ✅ Encriptar datos antes de enviarlos a la base de datos
					string telefonoEncriptado = model.Telefono != null ? EncryptionHelper.Encrypt(model.Telefono) : "";
					string tokenEncriptado = model.Token_verificacion != null ? EncryptionHelper.Encrypt(model.Token_verificacion) : "";
					string contrasenaEncriptada = EncryptionHelper.Encrypt(model.Contrasena);

					string respuesta1Encriptada = EncryptionHelper.Encrypt(model.Respuesta1);
					string respuesta2Encriptada = EncryptionHelper.Encrypt(model.Respuesta2);
					string respuesta3Encriptada = EncryptionHelper.Encrypt(model.Respuesta3);

					// Parámetros principales con datos encriptados
					cmd.Parameters.AddWithValue("@nombre", model.Nombre);
					cmd.Parameters.AddWithValue("@telefono", telefonoEncriptado);
					cmd.Parameters.AddWithValue("@correo", model.Correo);
					cmd.Parameters.AddWithValue("@edad", model.Edad ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@contrasena", contrasenaEncriptada);
					cmd.Parameters.AddWithValue("@token_verificacion", tokenEncriptado);

					// ✅ Agregar la dirección compuesta como parámetro
					cmd.Parameters.AddWithValue("@direccion", model.Direccion);

					// Parámetros de seguridad encriptados
					cmd.Parameters.AddWithValue("@respuesta1", respuesta1Encriptada);
					cmd.Parameters.AddWithValue("@respuesta2", respuesta2Encriptada);
					cmd.Parameters.AddWithValue("@respuesta3", respuesta3Encriptada);

					try
					{
						conn.Open();
						int result = cmd.ExecuteNonQuery();

						if (result > 0)
						{
							return RedirectToAction("Login", "Login");
						}
					}
					catch (SqlException ex)
					{
						// Manejo de errores específicos
						if (ex.Number == 2627)
						{
							ModelState.AddModelError("Correo", "El correo ya está registrado");
						}
						ModelState.AddModelError("", "Error de base de datos: " + ex.Message);
					}
				}
			}
		}
		return View("Login", model);
	}
}