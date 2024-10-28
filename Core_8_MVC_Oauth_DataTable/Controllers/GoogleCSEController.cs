using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Mvc;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class GoogleCSEController : Controller
	{
		private readonly IConfiguration _configuration;

		public GoogleCSEController(
			IConfiguration configuration
			)
		{

			_configuration = configuration;
			
		}




		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult redirect([FromForm] string search)
		{


			string YourGoogleID = _configuration.GetSection("GoogleCseId").Value!; //"YourGoogleID";
			string googleCsePath = $@"https://www.google.com/cse?cx={YourGoogleID}&ie=UTF-8&q={Uri.EscapeDataString(search)}";
	


			return Redirect(googleCsePath);
		}
	}
}
