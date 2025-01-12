using Core_8_MVC_Oauth_DataTable.Filter;
using Core_8_MVC_Oauth_DataTable.MiddleWare;
using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
			//// �s�u�r��榡�GServer=���A����m;Database=��Ʈw;User ID=�b��;Password=�K�X;TrustServerCertificate=true  // Windows ���ҡGTrusted_Connection=True
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
			builder.Services.AddHttpClient();  // ���U IHttpClientFactory
			builder.Services.AddScoped<CheckHaveOauthFilter>(); // �`�J Filter �A��


			// �[�J���ҼҦ� (Cookie �M �ĤT��n�J)
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				// options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
				options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // �K�[�o�@��A���w ForbidScheme
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // �]�m DefaultChallengeScheme
			})
			.AddCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.Lax;  // ���\�󯸽ШD�� Cookie
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // �ȾA�Ω� HTTPS
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;

                // �򥻰t�m�A�o�ǬO�q�Ϊ��t�m
                options.LoginPath = "/Home/Privacy";				// �n�J���|
				options.LogoutPath = "/Home/Privacy";			// �n�X���|
				options.AccessDeniedPath = "/Home/Privacy";


                // �۩w�q��󨤦⪺���|
                options.Events.OnRedirectToLogin = context =>
                {
                    var roles = context.HttpContext.User?.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

					// �ھڷ�e���ШD���|�M�w���w�V������
					var currentPath = context.HttpContext.Request.Path.Value;

					if (roles == null || !roles.Any()) // �S�����⪺���p�A�n������
					{
						Console.WriteLine("OnRedirectToLogin,null");
						//context.Response.Redirect("/Home/Privacy");

						//context.Response.Redirect("/Home/Privacy");
						Console.WriteLine("OnRedirectToAccessDenied,null");

						if (currentPath == null)
						{
							context.Response.Redirect("/Home/Privacy");
						}
						// �o�̧A�i�H�ھڸ��|�Ӷi�椣�P���B�z
						if (currentPath.Contains("/UserALoginDone"))
						{
							// �p�G�O�X�� Admin �����A���w�V��޲z���ڵ�����
							context.Response.Redirect("/Login");
						}
						else if (currentPath.Contains("/UserBLoginDone"))
						{
							// �p�G�O�X�ݥΤ᭶���A���w�V��Τ�ڵ�����
							context.Response.Redirect("/Home");
						}
						else
						{
							// �q�{���ڵ�����
							context.Response.Redirect("/Home/Privacy");
						}
					}
					else if (roles.Contains("A"))
					{
						// �p�G�Τ᪺����O "A"�A�h�ɦV�줣�P���n�J���|
						context.Response.Redirect("/Login");
					}
					else if (roles.Contains("B"))
					{
						// �p�G�Τ᪺����O "B"�A�h�ɦV�줣�P���n�J���|
						context.Response.Redirect("/Home");
					}
					else
					{
						Console.WriteLine("OnRedirectToLogin,else");
						// ��L���p�A�ϥιw�]���n�J����
						context.Response.Redirect("/Home/Privacy");
					}

					return Task.CompletedTask;
				};

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    var roles = context.HttpContext.User?.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

					// �ھڷ�e���ШD���|�M�w���w�V������
					var currentPath = context.HttpContext.Request.Path.Value;

					if (roles == null || !roles.Any()) // �S�����⪺���p�A�n������
					{
						//context.Response.Redirect("/Home/Privacy");
						Console.WriteLine("OnRedirectToAccessDenied,null");

						if (currentPath == null)
						{
							context.Response.Redirect("/Home/Privacy");
						}
						// �o�̧A�i�H�ھڸ��|�Ӷi�椣�P���B�z
						if (currentPath.Contains("/UserALoginDone"))
						{
							// �p�G�O�X�� Admin �����A���w�V��޲z���ڵ�����
							context.Response.Redirect("/Login");
						}
						else if (currentPath.Contains("/UserBLoginDone"))
						{
							// �p�G�O�X�ݥΤ᭶���A���w�V��Τ�ڵ�����
							context.Response.Redirect("/Home");
						}
						else
						{
							// �q�{���ڵ�����
							context.Response.Redirect("/Home/Privacy");
						}



					}
					else if (roles.Contains("A"))
					{
						// �p�G�Τ᪺����O "A"�A�h�ɦV�줣�P���n�J���|
						context.Response.Redirect("/Login");
					}
					else if (roles.Contains("B"))
					{
						// �p�G�Τ᪺����O "B"�A�h�ɦV�줣�P���n�J���|
						context.Response.Redirect("/Home");
					}
					else
					{

						Console.WriteLine("OnRedirectToAccessDenied,else");
						// ��L���p�A�ϥιw�]���n�J����
						context.Response.Redirect("/Home/Privacy");
					}

					return Task.CompletedTask;
				};



            })
			.AddGoogle(options =>
			{
				options.ClientId = builder.Configuration.GetSection("GoogleOAuth:CredentialId").Value!;                //"Your-Google-Client-Id";
				options.ClientSecret = builder.Configuration.GetSection("GoogleOAuth:CredentialSecret").Value!;  //"Your-Google-Client-Secret";
				options.CallbackPath = "/signin-google";  // google console �� callback ���}

			})
			.AddOAuth("Line", options =>
			{
				options.ClientId = builder.Configuration["LineOAuth:CredentialId"]!;
				options.ClientSecret = builder.Configuration["LineOAuth:CredentialSecret"]!;
				options.CallbackPath = new PathString("/signin-line");  // Line Dev �� callback ���}
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
						// �o��Log�ˬd�O�_���i��
						// Ping 

						// �ˬd�O�_�����T�� Access Token
						Console.WriteLine($"Access Token: {context.AccessToken}");

						// �o�e�ШD�� UserInformationEndpoint ����Τ᪺�򥻸��
						var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
						request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
						request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

						var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
						response.EnsureSuccessStatusCode();

						var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

						// ��� Line �ϥΪ� ID �M displayName
						var userId = user.RootElement.GetProperty("userId").GetString();
						var displayName = user.RootElement.GetProperty("displayName").GetString();

						// �T�O id_token �s�b
						var idToken = context.TokenResponse.Response.RootElement.GetProperty("id_token").GetString();
						if (idToken != null)
						{
							var handler = new JwtSecurityTokenHandler();
							var jwtToken = handler.ReadJwtToken(idToken);

							// �q id_token ������ email
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


							// �N�Τ��T�s�x�� context.Properties ��
							context.Properties.Items["email"] = emailClaim;
						}
						else
						{
							Console.WriteLine("id_token is missing.");
						}

						// ���L�Τ� ID �M displayName
						Console.WriteLine($"User ID: {userId}, Display Name: {displayName}");

						// �K�[�� Claims ��
						context.Identity.AddClaim(new Claim(ClaimTypes.Name, userId));
						context.Identity.AddClaim(new Claim("displayName", displayName));

						context.RunClaimActions(user.RootElement);

						await Task.CompletedTask;
					}
				};
			});


			// �[�J�S�w�� Roles
			//builder.Services.AddAuthorizationBuilder()
			//							// �[�J�S�w�� Roles
			//							.AddPolicy("�S�w��roles", policy => policy.RequireRole("roleName"));

			//builder.Services.AddAuthorization(options =>
			//{
			//	// �]�m��󨤦⪺���v����
			//	options.AddPolicy("RoleA", policy => policy.RequireRole("A"));
			//	options.AddPolicy("RoleB", policy => policy.RequireRole("B"));
			//});



			var app = builder.Build();

            // �b�����U�۩w�q�����n��
            app.UseMiddleware<ClientHintsMiddleware>();
            app.UseMiddleware<RemoveServerHeaderMiddleware>();


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

			app.UseAuthentication(); // �[�J����
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
