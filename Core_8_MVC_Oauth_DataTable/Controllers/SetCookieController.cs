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
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = true
            });

            return View();
        }
    }
}
