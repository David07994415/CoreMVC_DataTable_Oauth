using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace Core_8_MVC_Oauth_DataTable.MiddleWare
{
    public class RemoveServerHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        public RemoveServerHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 在回應發送前移除標頭(for Kestrel)
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");

                return Task.CompletedTask;
            });

            // 繼續處理請求
            await _next(context);

            // For IIS 可選則任一
            //1.使用 IIS 管理員設定
            //    打開 IIS 管理員。
            //    選中你的網站或伺服器節點。
            //    在右側的功能視窗中找到「HTTP 回應標頭」。
            //    點擊「移除」並選擇 Server。
            //2.在publish 的 web.config加入：
            //    < configuration >
            //      < system.webServer >
            //        < httpProtocol >
            //          < customHeaders >
            //            < remove name = "Server" />
            //          </ customHeaders >
            //        </ httpProtocol >
            //      </ system.webServer >
            //    </ configuration >

        }





    }
}
