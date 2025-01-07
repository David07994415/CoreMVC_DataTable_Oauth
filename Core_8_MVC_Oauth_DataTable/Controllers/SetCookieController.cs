using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
    public class SetCookieController : Controller
    {
        public IActionResult Index()
        {
            // 創建一個包含多個項目的字典
            var userData = new Dictionary<string, string>
            {
                { "AreaCode", "02" },
                { "DivCode", "01" }
            };

            // 將字典序列化為 JSON 字符串
            var json = JsonSerializer.Serialize(userData);

            // 設定 Cookie，將 JSON 字符串存儲在 "UserData" Cookie 中
            Response.Cookies.Append("CookieName", json, new CookieOptions
            {
				SameSite = SameSiteMode.Lax,  // 防止 CSRF 攻擊，根據需求選擇 Lax / Strict / None
				Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = true
            });

            return View();
        }


        public IActionResult ReadCookie()
		{
			// 從 Request 中讀取 Cookie
			var cookie = Request.Cookies["CookieName"];

			// 將 JSON 字符串反序列化為字典
			var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(cookie);

			// 讀取字典中的值
			var areaCode = userData["AreaCode"];
			var divCode = userData["DivCode"];

			return Content($"AreaCode: {areaCode}, DivCode: {divCode}");
		}

		public IActionResult DeleteCookie()
		{
			// 刪除 Cookie
			Response.Cookies.Delete("CookieName");

			return View();
		}
	}
}
