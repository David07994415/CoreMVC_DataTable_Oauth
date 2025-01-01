using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class AnitTokenController : Controller
	{
		public IActionResult Index()
		{
			
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult FormPost(AnitTokenViewModel input) 
		{
			AnitTokenViewModel output = new AnitTokenViewModel();
			output.name = input.name;
			output.email = input.email;
			return RedirectToAction("View");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult FormPost_Ajax(AnitTokenViewModel input)
		{
			AnitTokenViewModel output = new AnitTokenViewModel();
			output.name = input.name;
			output.email = input.email;
			return Ok(new { result ="pass"});
		}

	}

	public class AnitTokenViewModel 
	{
        public string name { get; set; }
		public string email { get; set; }
	}
}
