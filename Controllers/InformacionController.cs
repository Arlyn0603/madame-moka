using Microsoft.AspNetCore.Mvc;

namespace madame_moka.Controllers
{
	public class InformacionController : Controller
	{
		public IActionResult Informacion()
		{
			return View();
		}
	}
}
