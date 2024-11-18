using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Core_8_MVC_Oauth_DataTable.Filter
{
	public class CheckHaveOauthFilter : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context)
		{
			var user = context.HttpContext.User;

			if (user.Identity != null && user.Identity.IsAuthenticated)
			{
				// 取得指定的 Claim
				var specificClaim = user.FindFirst("YourClaimType"); // 替換 "YourClaimType" 為你的 Claim 類型

				if (specificClaim != null && specificClaim.Value == "SomeCondition") // 根據某種條件判斷
				{
					// 進行重新導向
					context.Result = new RedirectToActionResult("Index", "Home", null);
				}
			}
			else
			{
				// 處理未經驗證的使用者
				context.Result = new RedirectToActionResult("Login", "Account", null);
			}
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			throw new NotImplementedException();
		}
	}
}
