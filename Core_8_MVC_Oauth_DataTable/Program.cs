using Core_8_MVC_Oauth_DataTable.Filter;
using Core_8_MVC_Oauth_DataTable.MiddleWare;
using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Core_8_MVC_Oauth_DataTable
{
	public class Program
	{
		public static void Main(string[] args)
		{

			//		"ConnectionStrings": {
			//			"coreDbConString": "Server=.;Database=coredb;Trusted_Connection=True;TrustServerCertificate=true"
			//// 連線字串格式：Server=伺服器位置;Database=資料庫;User ID=帳號;Password=密碼;TrustServerCertificate=true  // Windows 驗證：Trusted_Connection=True
			// },
			// "GoogleOAuth": {
			//			"CredentialId": "your ID",
			//   "CredentialSecret": "your secret"
			// },
			// "LineOAuth": {
			//			"CredentialId": "your ID",
			//   "CredentialSecret": "your Secret"
			// }


			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<coredbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("coreDbConString")));
			builder.Services.AddHttpClient();  // 註冊 IHttpClientFactory
			builder.Services.AddScoped<CheckHaveOauthFilter>(); // 注入 Filter 服務


			// 加入驗證模式 (Cookie 和 第三方登入)
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				// options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.Lax;  // 允許跨站請求的 Cookie
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 僅適用於 HTTPS
				options.LoginPath = "/Login/Index";				// 登入路徑
				options.LogoutPath = "/Login/Index";			// 登出路徑
				options.AccessDeniedPath = "/Login/Index"; 
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				options.SlidingExpiration = true;

			})
			.AddGoogle(options =>
			{
				options.ClientId = builder.Configuration.GetSection("GoogleOAuth:CredentialId").Value!;                //"Your-Google-Client-Id";
				options.ClientSecret = builder.Configuration.GetSection("GoogleOAuth:CredentialSecret").Value!;  //"Your-Google-Client-Secret";
				options.CallbackPath = "/signin-google";  // google console 的 callback 網址

			})
			.AddOAuth("Line", options =>
			{
				options.ClientId = builder.Configuration["LineOAuth:CredentialId"]!;
				options.ClientSecret = builder.Configuration["LineOAuth:CredentialSecret"]!;
				options.CallbackPath = new PathString("/signin-line");  // Line Dev 的 callback 網址
				options.AuthorizationEndpoint = "https://access.line.me/oauth2/v2.1/authorize";
				options.TokenEndpoint = "https://api.line.me/oauth2/v2.1/token";
				options.UserInformationEndpoint = "https://api.line.me/v2/profile";

				options.Scope.Add("profile");
				options.Scope.Add("openid");
				options.Scope.Add("email");

				options.SaveTokens = true;

				options.Events = new OAuthEvents
				{
					OnCreatingTicket = async context =>
					{
						// 檢查是否有正確的 Access Token
						Console.WriteLine($"Access Token: {context.AccessToken}");

						// 發送請求到 UserInformationEndpoint 獲取用戶的基本資料
						var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
						request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
						request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

						var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
						response.EnsureSuccessStatusCode();

						var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

						// 獲取 Line 使用者 ID 和 displayName
						var userId = user.RootElement.GetProperty("userId").GetString();
						var displayName = user.RootElement.GetProperty("displayName").GetString();

						// 確保 id_token 存在
						var idToken = context.TokenResponse.Response.RootElement.GetProperty("id_token").GetString();
						if (idToken != null)
						{
							var handler = new JwtSecurityTokenHandler();
							var jwtToken = handler.ReadJwtToken(idToken);

							// 從 id_token 中提取 email
							var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
							if (emailClaim != null)
							{
								context.Identity.AddClaim(new Claim(ClaimTypes.Email, emailClaim));
								Console.WriteLine($"Email: {emailClaim}");
							}
							else
							{
								Console.WriteLine("No email provided in id_token.");
							}


							// 將用戶資訊存儲到 context.Properties 中
							context.Properties.Items["email"] = emailClaim;
						}
						else
						{
							Console.WriteLine("id_token is missing.");
						}

						// 打印用戶 ID 和 displayName
						Console.WriteLine($"User ID: {userId}, Display Name: {displayName}");

						// 添加到 Claims 中
						context.Identity.AddClaim(new Claim(ClaimTypes.Name, userId));
						context.Identity.AddClaim(new Claim("displayName", displayName));

						context.RunClaimActions(user.RootElement);

						await Task.CompletedTask;
					}
				};
			});


			// 加入特定的 Roles
			builder.Services.AddAuthorizationBuilder()
										// 加入特定的 Roles
										.AddPolicy("特定的roles", policy => policy.RequireRole("roleName"));




			var app = builder.Build();

            // 在此註冊自定義中介軟體
            app.UseMiddleware<ClientHintsMiddleware>();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication(); // 加入驗證
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
