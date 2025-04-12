using madame_moka.Data;
using Microsoft.AspNetCore.Mvc;

namespace MadameMoka.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var usuarios = _context.Usuarios.ToList();
			return View(usuarios);
		}
	}
}
