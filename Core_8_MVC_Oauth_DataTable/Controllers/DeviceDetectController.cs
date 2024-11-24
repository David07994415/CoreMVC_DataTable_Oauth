using Core_8_MVC_Oauth_DataTable.Migrations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
    public class DeviceDetectController : Controller
    {
        public IActionResult Index()
        {
            // User-Agent Client Hints
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-CH-UA

            // Add MiddleWare：ClientHintsMiddleware
            // Program Add  MiddleWare

            // Check Context
            var context = HttpContext;

            string User_Agent_String = context.Request.Headers["User-Agent"];

            if (!User_Agent_String.Contains("Mobile")) // 非手機裝置使用者
            {
                // 可決定是否要 Return
            }

            // 優先嘗試透過 Client Hints 取得設備型號
            if (context.Request.Headers.ContainsKey("Sec-CH-UA-Model"))
            {
                var deviceModel = context.Request.Headers["Sec-CH-UA-Model"].ToString();

                string deviceModel_Replace = deviceModel.Replace("\"", "");
                if (!string.IsNullOrEmpty(deviceModel_Replace))  // Note：空白會返回 ""，可以用 replace() 先取代
                {
                   
                    string DeviceModel = deviceModel_Replace; // 取得使用者手機型號
                }
            }
            else  // 沒有支援 Client Hints， 可以使用套件(UAParse) 或者正規表達式
            {
                // Iphone：
                // Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/537.36 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/537.36

                // Android 
                // Mozilla/5.0 (Linux; Android 10; Pixel 4 XL Build/QD1A.190805.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Mobile Safari/537.36

                // 使用正則表達式來匹配並取得第一個括號內的內容
                var match = Regex.Match(User_Agent_String, @"\((.*?)\)");

                if (match.Success)
                {
                    string matching_String = match.Groups[1].Value; // 返回第一組匹配的內容（括號內的部分）
                }

                // 使用正則表達式來匹配 "Mobile/" 後面的字母和數字
                var match2 = Regex.Match(User_Agent_String, @"Mobile\/([A-Za-z0-9]+)");

                if (match2.Success)
                {
                    string matching_String2 = match2.Groups[1].Value; // 返回第一組匹配的內容（括號內的部分）

                }

            }

           

            return View();
        }
    }
}
