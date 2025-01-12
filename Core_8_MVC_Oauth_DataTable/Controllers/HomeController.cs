using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
			var error = exception?.Error;

			// ��x�O�����`�]�p�G�ݭn�^
			_logger.LogError(error, "Unhandled exception occurred");

			// �N���~�ǻ������
			return View(error);


			// return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
