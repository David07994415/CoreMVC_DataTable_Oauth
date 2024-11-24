namespace Core_8_MVC_Oauth_DataTable.MiddleWare
{
    public class ClientHintsMiddleware
    {

        private readonly RequestDelegate _next;

        public ClientHintsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 設置 Accept-CH 頭部
            context.Response.Headers["Accept-CH"] 
                = "Sec-CH-UA, Sec-CH-UA-Mobile, Sec-CH-UA-Platform,Sec-CH-UA-Model";

            // 繼續處理請求
            await _next(context);
        }
    }
}
