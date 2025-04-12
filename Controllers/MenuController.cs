using Microsoft.AspNetCore.Mvc;

namespace madame_moka.Controllers
{
	
    public class MenuController : Controller
	{
		public IActionResult Menu()
		{
			return View("menu"); // 🔹 Cargar la vista menu.cshtml en lugar de Index.cshtml
		}
	}
}

