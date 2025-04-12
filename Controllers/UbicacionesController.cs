using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using madame_moka.Data;
using madame_moka.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace madame_moka.Controllers
{
	[Route("Ubicaciones")]
	public class UbicacionesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<UbicacionesController> _logger;

		public UbicacionesController(ApplicationDbContext context, ILogger<UbicacionesController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet("ObtenerPaises")]
		public async Task<IActionResult> ObtenerPaises()
		{
			try
			{
				var paises = await _context.Ubicaciones
					.Where(u => u.Tipo == "Pais")
					.OrderBy(u => u.Nombre)
					.Select(u => new { id = u.Id, nombre = u.Nombre }) 
					.ToListAsync();

				return Json(paises);
			}
			catch (Exception ex)
			{
				_logger.LogError($"❌ ERROR en ObtenerPaises: {ex.Message}\n{ex.StackTrace}");
				return StatusCode(500, new { success = false, message = $"Error en el servidor: {ex.Message}" });
			}
		}


		[HttpGet("ObtenerProvincias")]
		public async Task<IActionResult> ObtenerProvincias(int idPais)
		{
			try
			{
				_logger.LogInformation($" Buscando provincias para el país con id: {idPais}");

				var provincias = await _context.Ubicaciones
					.Where(u => u.Tipo == "Provincia" && u.IdPadre == idPais)
					.OrderBy(u => u.Nombre)
					.Select(u => new { Id = u.Id, Nombre = u.Nombre }) // Asegurar nombres correctos
					.ToListAsync();

				if (!provincias.Any())
				{
					_logger.LogWarning($" No se encontraron provincias para el país con id: {idPais}");
				}

				return Json(provincias);
			}
			catch (Exception ex)
			{
				_logger.LogError($" ERROR en ObtenerProvincias: {ex.Message}\n{ex.StackTrace}");
				return StatusCode(500, new { success = false, message = $"Error en el servidor: {ex.Message}" });
			}
		}


		// Obtener cantones por provincia
		[HttpGet("ObtenerCantones")]
		public async Task<IActionResult> ObtenerCantones(int idProvincia)
		{
			try
			{
				_logger.LogInformation($"Buscando cantones para la provincia ID: {idProvincia}");

				var cantones = await _context.Ubicaciones
					.Where(u => u.Tipo == "Canton" && u.IdPadre == idProvincia)
					.OrderBy(u => u.Nombre)
					.Select(u => new { Id = u.Id, Nombre = u.Nombre })
					.ToListAsync();

				if (!cantones.Any())
				{
					_logger.LogWarning($" No se encontraron cantones para la provincia ID: {idProvincia}");
				}

				return Json(cantones);
			}
			catch (Exception ex)
			{
				_logger.LogError($" ERROR en ObtenerCantones: {ex.Message}\n{ex.StackTrace}");
				return StatusCode(500, new { success = false, message = "Error interno en el servidor." });
			}
		}




		[HttpGet("ObtenerDistritos")]
		public async Task<IActionResult> ObtenerDistritos(int idCanton)
		{
			try
			{
				var distritos = await _context.Ubicaciones
					.Where(u => u.Tipo == "Distrito" && u.IdPadre == idCanton)
					.OrderBy(u => u.Nombre)
					.Select(u => new { u.Id, u.Nombre }) 
					.ToListAsync();

				return Json(distritos);
			}
			catch (Exception ex)
			{
				Console.WriteLine($" Error en ObtenerDistritos: {ex}");
				return StatusCode(500, new { success = false, message = "Error interno en el servidor." });
			}
		}

	}
}
