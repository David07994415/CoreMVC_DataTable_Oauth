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
using Core_8_MVC_Oauth_DataTable.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using static System.Formats.Asn1.AsnWriter;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class LoginController : Controller
	{

		// private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;
		private readonly coredbContext _db;


		public LoginController(
			//IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			coredbContext db,
			HttpClient httpClient)
		{
			// _httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_db = db;
			_httpClient = httpClient;
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
			ViewBag.Account = account;
			ViewBag.Password = pw;

			// 基本驗證
			if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(pw))
			{
				ViewBag.Message = "帳號密碼為空";
				return View("Index");
			}

			// 會員表單資料表驗證(額外：需要確認是現任、並且解密pid)
			var memberData = _db.MemberShip.Where(x => x.Pid == account && x.Phone == pw).FirstOrDefault();
			if (memberData == null)
			{
				ViewBag.Message = "非現任會員";
				return View("Index");
			}
			// 有這個現任會員

			// 取得會員表單 Sn
			int memberShip_Sn = memberData.SN;

			// 第三方資料表驗證，查看是否存在
			var OAuthData = _db.OAuthTable.Where(x => x.MemberShIpSn == memberShip_Sn).FirstOrDefault();
			if (OAuthData == null) //新創立
			{
				OAuthTable InserData = new OAuthTable();
				InserData.MemberShIpSn = memberShip_Sn;
				InserData.IsPhoneVerify = false;
				InserData.GoogleId = null;
				InserData.RebindGoogle = null;
				InserData.LineId = null;
				InserData.RebindLine = null;
				InserData.CreateDate = DateTime.Now;
				InserData.UpdateDate = DateTime.Now;
				InserData.LoginDate = null;
				InserData.IsExpired = false;

				_db.OAuthTable.Add(InserData);
				_db.SaveChanges();
			}


			var OAuthData_Check = _db.OAuthTable.Where(x => x.MemberShIpSn == memberShip_Sn).FirstOrDefault();

			if (OAuthData_Check == null)
			{
				ViewBag.Message = "找不到會員，有問題";
				return View("Index");
			}

			if (OAuthData_Check.IsExpired == true)
			{
				ViewBag.Message = "會員已經過期，請與管理人員聯絡";
				return View("Index");
			}

			if (OAuthData_Check.IsPhoneVerify == false)
			{
				ViewBag.Message = "請進行電話SMS驗證";
				ViewBag.VerifySmsPhone = true;
				return View("Index");
			}
			else if (OAuthData_Check.IsPhoneVerify == true)  // 如果已經驗證過了，可以進行第三方登入
			{
				ViewBag.Message = "請進行第三方登入";
				ViewBag.ThreePartyLogin = true;
				return View("Index");
			}





			return View("Index");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CheckSmsPhone([FromForm] CheckSmsDto Input)  // 這邊要進行電話驗證
		{
			string account = Input.Account;
			string pw = Input.Password;
			ViewBag.Account = account;
			ViewBag.Password = pw;
			ViewBag.VerifySmsPhone = false;

			// 基本驗證
			if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(pw))
			{
				ViewBag.Message = "帳號密碼為空";
				return View("Index");
			}

			// 會員表單資料表驗證(額外：需要確認是現任、並且解密pid)
			var memberData = _db.MemberShip.Where(x => x.Pid == account && x.Phone == pw).FirstOrDefault();
			if (memberData == null)
			{
				ViewBag.Message = "非現任會員";
				return View("Index");
			}
			// 有這個現任會員

			// 取得會員表單 Sn
			int memberShip_Sn = memberData.SN;

			// 第三方資料表驗證，查看是否存在
			var OAuthData = _db.OAuthTable.Where(x => x.MemberShIpSn == memberShip_Sn).FirstOrDefault();
			if (OAuthData == null)
			{
				ViewBag.Message = "請重新登入";
				return View("Index");
			}

			if (OAuthData.IsExpired == true)
			{
				ViewBag.Message = "會員已經過期，請與管理人員聯絡";
				return View("Index");
			}

			if (OAuthData.IsPhoneVerify == true)
			{
				ViewBag.Message = "請進行第三方登入";
				ViewBag.ThreePartyLogin = true;
				return View("Index");
			}
			// 如果沒有電話驗證

			if (string.IsNullOrEmpty(Input.SmsCode))
			{
				ViewBag.Message = "請輸入驗證碼";
				ViewBag.VerifySmsPhone = true;
				return View("Index");
			}

			if (Input.SmsCode == "Code") // 驗證碼通過
			{
				OAuthData.IsPhoneVerify = true;
				_db.SaveChanges();

				ViewBag.Message = "請進行第三方登入";
				ViewBag.ThreePartyLogin = true;
				return View("Index");
			}
			else
			{
				ViewBag.VerifySmsPhone = true;
				ViewBag.Message = "驗證碼輸入錯誤，請重新輸入";
				return View("Index");
			}


			// return View("Index");
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

				// 判斷設備是否為手機，進而決定是否使用 Line App 的 Deep Link
				var isMobile = Request.Headers["User-Agent"].ToString().Contains("Mobile");
				string lineOAuthUrl;

				if (isMobile)
				{
					string clientId = _configuration["LineOAuth:CredentialId"];
					string redirectUri = Url.Action("LineResponseMobile", "Login", null, Request.Scheme);
					string state = Guid.NewGuid().ToString();
					string scope = "openid profile email";



					// 手機設備使用 Line App 的 Deep Link
					lineOAuthUrl = $"line://oauth2/authorize?response_type=code" +
								   $"&client_id={clientId}" +
								   $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
								   $"&state={state}" +
								   $"&scope={scope}";
					return Redirect(lineOAuthUrl); // 直接重定向到 Line App
				}
				else
				{
					var redirectUrl = Url.Action("LineResponse", "Login");  // LineResponse  LineResponse_Check
					var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
					return Challenge(properties, "Line");
				}
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


		// deep link ：需要製作一個view
		[HttpGet]
		public async Task<IActionResult> LineResponseMobile(string code, string state)
		{
			if (string.IsNullOrEmpty(code))
			{
				return RedirectToAction("Index");
			}

			// Step 1: 使用授權碼交換 Access Token
			string clientId = _configuration["LineOAuth:CredentialId"];
			string clientSecret = _configuration["LineOAuth:CredentialSecret"];
			string redirectUri = Url.Action("LineResponse", "Login", null, Request.Scheme);

			var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token");
			var tokenRequestContent = new FormUrlEncodedContent(new Dictionary<string, string>
	{
		{ "grant_type", "authorization_code" },
		{ "code", code },
		{ "redirect_uri", Uri.EscapeDataString(redirectUri) },
		{ "client_id", clientId },
		{ "client_secret", clientSecret }
	});

			tokenRequest.Content = tokenRequestContent;
			var tokenResponse = await _httpClient.SendAsync(tokenRequest);

			if (!tokenResponse.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}

			// 解析 Access Token
			var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
			var tokenData = JsonDocument.Parse(tokenResponseContent);
			string accessToken = tokenData.RootElement.GetProperty("access_token").GetString();

			// Step 2: 使用 Access Token 獲取用戶資料
			var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/v2/profile");
			userRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
			var userResponse = await _httpClient.SendAsync(userRequest);

			if (!userResponse.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}

			var userResponseContent = await userResponse.Content.ReadAsStringAsync();
			var userData = JsonDocument.Parse(userResponseContent);
			string userId = userData.RootElement.GetProperty("userId").GetString();
			string displayName = userData.RootElement.GetProperty("displayName").GetString();
			string email = userData.RootElement.GetProperty("email").GetString();

			// Step 3: 註冊或登入邏輯
			// 這裡可以使用 userId 和 email 確認用戶資料
			var dbContext = ...; // 獲取資料庫上下文
			var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.LineUserId == userId);
			if (existingUser == null)
			{
				// 創建新用戶或進行其他處理
			}

			// Step 4: 進行認證或其他處理
			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.NameIdentifier, userId),
		new Claim(ClaimTypes.Name, displayName),
		new Claim(ClaimTypes.Email, email)
	};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return RedirectToAction("Index", "Home");
		}


		//[Authorize(Policy = "特定的roles")]


		//// 建立 Claims
		//var claims = new List<Claim>
		//		{
		//			new Claim(ClaimTypes.Name, user.Username),
		//			new Claim(ClaimTypes.Role, "UserRole"), // 可以根據用戶的角色進行調整
		//                  // 其他需要的 claims
		//              };

		//var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		//var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

		//// 登入並生成 Cookie
		//await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// 沒用到
		/// </summary>
		/// <returns></returns>
		// [HttpGet("signin-line")]
		//public async Task<IActionResult> LineResponse_Check()
		//{
		//	// 從請求中獲取授權碼
		//	var code = Request.Query["code"];
		//	if (string.IsNullOrEmpty(code))
		//	{
		//		return BadRequest("Authorization code not provided");
		//	}

		//	// 構建獲取 token 的請求
		//	var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.line.me/oauth2/v2.1/token")
		//	{
		//		Content = new FormUrlEncodedContent(new Dictionary<string, string>
		//	{
		//		{ "grant_type", "authorization_code" },
		//		{ "code", code },
		//		{ "redirect_uri", "https://localhost:7240/signin-line" }, // 必須與註冊 Line 登入時的回調 URL 一致
		//              { "client_id", _configuration.GetSection("LineOAuth:CredentialId").Value },
		//		{ "client_secret", _configuration.GetSection("LineOAuth:CredentialSecret").Value }
		//		})
		//	};



		//	var client = _httpClientFactory.CreateClient();
		//	var tokenResponse = await client.SendAsync(tokenRequest);
		//	if (!tokenResponse.IsSuccessStatusCode)
		//	{
		//		return BadRequest("Error retrieving access token");
		//	}

		//	var tokenResult = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());

		//	var accessToken = tokenResult.RootElement.GetProperty("access_token").GetString();
		//	var idToken = tokenResult.RootElement.GetProperty("id_token").GetString();

		//	// 使用 access_token 獲取用戶資料
		//	var profileRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/v2/profile");
		//	profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		//	var profileResponse = await client.SendAsync(profileRequest);
		//	if (!profileResponse.IsSuccessStatusCode)
		//	{
		//		return BadRequest("Error retrieving user profile");
		//	}

		//	var profileResult = JsonDocument.Parse(await profileResponse.Content.ReadAsStringAsync());

		//	// 解析用戶信息
		//	var userId = profileResult.RootElement.GetProperty("userId").GetString();
		//	var displayName = profileResult.RootElement.GetProperty("displayName").GetString();

		//	// 使用 id_token 解碼並提取 email
		//	var handler = new JwtSecurityTokenHandler();
		//	var jwtToken = handler.ReadJwtToken(idToken);
		//	var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

		//	// 處理用戶資料（儲存到 session，或者直接登入）
		//	// ...

		//	return RedirectToAction("Index", "Home");
		//}



		public async Task LogoutBtn()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
		public async Task<ActionResult> UserALogin()
		{
			// 建立 Claims
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "UserA"),
				new Claim(ClaimTypes.Role, "A"), // 可以根據用戶的角色進行調整
			     // 其他需要的 claims
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			// 登入並生成 Cookie
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

			return RedirectToAction("UserALoginDone");
		}


		public async Task<ActionResult> UserBLogin()
		{
			// 建立 Claims
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "UserB"),
				new Claim(ClaimTypes.Role, "B"), // 可以根據用戶的角色進行調整
			     // 其他需要的 claims
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			// 登入並生成 Cookie
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

			return RedirectToAction("UserBLoginDone");
		}


		[Authorize(Roles = "A")]
		public async Task<ActionResult> UserALoginDone()
		{
			var UserIdentity = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();

			if (UserIdentity == null)
			{
				ViewBag.UserType = "沒有資料";
			}
			else
			{
				ViewBag.UserType = UserIdentity;
			}

			return View();
		}

		[Authorize(Roles = "B")]
		public async Task<ActionResult> UserBLoginDone()
		{
			var UserIdentity = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();

			if (UserIdentity == null)
			{
				ViewBag.UserType = "沒有資料";
			}
			else
			{
				ViewBag.UserType = UserIdentity;
			}

			return View();
		}



	}
}
