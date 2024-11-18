using Core_8_MVC_Oauth_DataTable.Filter;
using Microsoft.AspNetCore.Mvc;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	[ServiceFilter(typeof(CheckHaveOauthFilter))] // 需要再 Program 中 注入
	public class FilterController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
