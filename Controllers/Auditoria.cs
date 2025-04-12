using Microsoft.AspNetCore.Mvc;
using madame_moka.Models;
using madame_moka.Services;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace madame_moka.Controllers
{
	[Route("api/auditoria")]
	[ApiController]
	public class AuditoriaController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IAuditoriaService _auditoriaService;

		public AuditoriaController(IConfiguration configuration, IAuditoriaService auditoriaService)
		{
			_configuration = configuration;
			_auditoriaService = auditoriaService;
		}

		[HttpGet("ObtenerAuditorias")]
		public ActionResult<List<Auditoria>> ObtenerAuditorias()
		{
			var auditorias = _auditoriaService.ObtenerAuditorias();
			return Ok(auditorias);
		}

		[HttpPost("RegistrarMovimiento")]
		public ActionResult RegistrarMovimiento([FromBody] Auditoria movimiento)
		{
			try
			{
				Console.WriteLine($"Recibiendo auditoría: {movimiento.IdUsuario}, {movimiento.Movimiento}, {movimiento.Descripcion}");
				_auditoriaService.RegistrarMovimiento(movimiento.IdUsuario, movimiento.Movimiento, movimiento.Descripcion);
				return Ok(new { success = true, message = "✅ Movimiento registrado" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = $"❌ Error en auditoría: {ex.Message}" });
			}
		}


	}
}
