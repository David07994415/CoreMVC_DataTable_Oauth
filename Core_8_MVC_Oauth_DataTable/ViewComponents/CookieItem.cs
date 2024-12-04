using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Core_8_MVC_Oauth_DataTable.ViewComponents
{
    public class CookieItem: ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var data = await _apiList.GetApiConnectionData();
            //// 預設資料夾：/Views/Shared/Components/{檢視元件名稱}/{檢視名稱}

            //if (TestParam != null)
            //{
            //    ViewBag.TestPara = TestParam;
            //}

            // 讀取 Cookie 並反序列化為字典
            var userDataJson = Request.Cookies["CookieName"];
            if (userDataJson != null)
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(userDataJson);

                // 顯示 Cookie 的值
                ViewBag.CookieContent = $"UserId: {userData["AreaCode"]}, UserName: {userData["DivCode"]}";
            }
            else
            {
                ViewBag.CookieContent = "No user data found in cookie.";
            }



            return View();
        }


    }
}
