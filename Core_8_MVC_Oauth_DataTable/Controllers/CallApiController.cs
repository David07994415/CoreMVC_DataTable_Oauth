using Core_8_MVC_Oauth_DataTable.Dtos;
using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class CallApiController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public CallApiController(HttpClient httpClient,IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
		}


		public IActionResult Index()
		{
			return View();
		}



		[NonAction]
		public async Task<IActionResult> SendSms(string MobileNumber, string RandomNumber,string EncodePid)
		{
			var apiUrl = "https://api.example.com/sms/send";
			var username = _configuration.GetSection("SmsApi:Account").Value; // 申請的帳號
			var password = _configuration.GetSection("SmsApi:Password").Value; // 申請的密碼

			// 設定 HTTP Basic Authentication
			var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

			// 設定 Content-Type
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// 準備請求資料
			DateTime dt = DateTime.Now;
			string dtId = dt.ToString("yyyyMMddHHmmssffff");
			var requestDto = new SmsApiRequestDto
			{
				orderNo = dtId,
				packageNo = "1",
				packageCnt = "1",
			};

			var requestTextBodyDto = new SmsApiRequestBodyDto()
			{
				uid = dtId,
				mobile = MobileNumber,
				smsText = $"第一行要加上機關名稱，您的驗證碼為：{RandomNumber}"
			};
			requestDto.smsBody.Add(requestTextBodyDto);

			// 建立儲存物件
			SmsLog NewSendSmsLog = new SmsLog()
			{
				uid=dtId,
				pid = EncodePid,
				Code=RandomNumber,
				hasVerify = false,
				SendTime = DateTime.Now
			};


			// 序列化資料為 JSON 格式
			var content = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");

			// 發送 POST 請求
			var response = await _httpClient.PostAsync(apiUrl, content);

			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var smsResponse = JsonSerializer.Deserialize<SmsApiResponseDto>(jsonResponse);

				if (smsResponse == null) 
				{
					// Log紀錄 
					// return
				}

				// 回應資料處理，例如檢查 status 或記錄錯誤明細
				if (smsResponse.status == "0000")
				{
					NewSendSmsLog.IsSendSuccess = true;

					// Db Add // SaveChange()
				}
				else 
				{
					NewSendSmsLog.IsSendSuccess = false;
					string statusMsg = smsResponse.statusMsg;
					string uidStatusMsg = "";
					if (smsResponse.errorUids.Count > 0)
					{
						uidStatusMsg = smsResponse.errorUids[0].uidStatusMsg;
					}
					NewSendSmsLog.ErrorMessage = statusMsg+";"+uidStatusMsg;

					// Db Add // SaveChange()
				}

				// 驗證邏輯
				List<SmsLog> ListofSmsLog = new List<SmsLog>();
				var CheckSmsLog = ListofSmsLog.Where(x=>x.pid== EncodePid && x.IsSendSuccess==true && x.hasVerify==false).OrderByDescending(x=>x.SendTime).FirstOrDefault();
				if (CheckSmsLog == null)
				{
					// 請重新發送簡訊
					CheckSmsLog.VerifyTime = DateTime.Now;
					// SaveChange()
				}
				DateTime dtMinus = DateTime.Now.AddMinutes(-5);
				if (CheckSmsLog.SendTime < dtMinus)
				{
					// 請重新發送簡訊
					CheckSmsLog.VerifyTime = DateTime.Now;
					// SaveChange()
				}

				if (CheckSmsLog.Code != RandomNumber)
				{
					// 驗證碼輸入錯誤
					CheckSmsLog.VerifyTime = DateTime.Now;
					// SaveChange()
				}
				else 
				{
					CheckSmsLog.hasVerify = true;
					CheckSmsLog.VerifyTime = DateTime.Now;
					// SaveChange()
				}





			}
			else 
			{
				// Log紀錄
				// return
			}

			return StatusCode((int)response.StatusCode, "Failed to send SMS.");
		}

	}
}
