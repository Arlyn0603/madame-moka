using Microsoft.AspNetCore.Mvc;

namespace madame_moka.Controllers
{
	public class UsuarioController : Controller
	{
		public IActionResult Usuario()
		{
			return View("usuario");
		}
	}
}
