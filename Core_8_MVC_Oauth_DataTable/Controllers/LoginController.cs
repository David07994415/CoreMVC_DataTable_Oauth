using Core_8_MVC_Oauth_DataTable.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class LoginController : Controller
	{

		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public LoginController(IHttpClientFactory httpClientFactory,IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login([FromForm] LoginDto Input)  // 傳統登入
		{

			string account = Input.Account;
			string pw = Input.Password;

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> OAuthSignIn(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return RedirectToAction("Index");
			}

			if (path == "GoogleResponse")
			{

				var redirectUrl = Url.Action("SigninGoogle", "Login");
				var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
				return Challenge(properties, GoogleDefaults.AuthenticationScheme);
			}
			else if (path == "LineResponse")
			{
				var redirectUrl = Url.Action("LineResponse", "Login");  // LineResponse  LineResponse_Check
				var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
				return Challenge(properties, "Line");
			}

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> SigninGoogle()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (result?.Principal == null)
			{
				return RedirectToAction("Index");
			}

			var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
			var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			// 這裡可以依據 email 做使用者註冊或登入邏輯
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> LineResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (result?.Principal == null)
			{
				return RedirectToAction("Index");
			}

			if (!result.Principal.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index");
			}


			var userEmail = result.Properties.Items["email"];
			int test = 1;

			//// 這邊進行資料庫會員確認
			//var dbContext = ...; // 獲取資料庫上下文
			//var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.LineUserId == userId);
			//if (existingUser == null)
			//{
			//	// 創建新用戶或進行其他處理
			//}

			//var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
			//var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
			//var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;


			// context.Identity.AddClaim(new Claim(ClaimTypes.Email, emailClaim));

			// 這裡可以依據 email 做使用者註冊或登入邏輯
			return RedirectToAction("Index", "Home");
		}


		// [HttpGet("signin-line")]
		public async Task<IActionResult> LineResponse_Check()
		{
			// 從請求中獲取授權碼
			var code = Request.Query["code"];
			if (string.IsNullOrEmpty(code))
			{
				return BadRequest("Authorization code not provided");
			}

			// 構建獲取 token 的請求
			var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token")
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "grant_type", "authorization_code" },
				{ "code", code },
				{ "redirect_uri", "https://localhost:7240/signin-line" }, // 必須與註冊 Line 登入時的回調 URL 一致
                { "client_id", _configuration.GetSection("LineOAuth:CredentialId").Value },
				{ "client_secret", _configuration.GetSection("LineOAuth:CredentialSecret").Value }
				})
			};



			var client = _httpClientFactory.CreateClient();
			var tokenResponse = await client.SendAsync(tokenRequest);
			if (!tokenResponse.IsSuccessStatusCode)
			{
				return BadRequest("Error retrieving access token");
			}

			var tokenResult = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());

			var accessToken = tokenResult.RootElement.GetProperty("access_token").GetString();
			var idToken = tokenResult.RootElement.GetProperty("id_token").GetString();

			// 使用 access_token 獲取用戶資料
			var profileRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/v2/profile");
			profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var profileResponse = await client.SendAsync(profileRequest);
			if (!profileResponse.IsSuccessStatusCode)
			{
				return BadRequest("Error retrieving user profile");
			}

			var profileResult = JsonDocument.Parse(await profileResponse.Content.ReadAsStringAsync());

			// 解析用戶信息
			var userId = profileResult.RootElement.GetProperty("userId").GetString();
			var displayName = profileResult.RootElement.GetProperty("displayName").GetString();

			// 使用 id_token 解碼並提取 email
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(idToken);
			var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

			// 處理用戶資料（儲存到 session，或者直接登入）
			// ...

			return RedirectToAction("Index", "Home");
		}



	}
}
