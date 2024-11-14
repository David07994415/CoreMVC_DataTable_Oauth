using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Serialization;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class XmlApiController : Controller
	{
		public async Task<IActionResult> Index()
		{
			object requestData = new object(); // your input
			var response = await CallApiWithXmlAsync<ResponseModel>(requestData);
			Console.WriteLine($"Status: {response.Status}, Message: {response.Message}");


			return View();
		}

		public async Task<T> CallApiWithXmlAsync<T>(object requestData) where T : class
		{
			using (HttpClient client = new HttpClient())
			{
				// 設置 API URL
				string apiUrl = "https://example.com/api";

				// 序列化請求資料為 XML
				XmlSerializer requestSerializer = new XmlSerializer(requestData.GetType());
				StringBuilder xmlStringBuilder = new StringBuilder();
				using (var writer = new System.IO.StringWriter(xmlStringBuilder))
				{
					requestSerializer.Serialize(writer, requestData);
				}
				string xmlContent = xmlStringBuilder.ToString();

				// 設置 HttpContent
				HttpContent content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");

				// 設置接收格式為 XML
				client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

				// 發送 POST 請求
				HttpResponseMessage response = await client.PostAsync(apiUrl, content);

				if (response.IsSuccessStatusCode)
				{
					// 讀取回應內容為字串
					string responseContent = await response.Content.ReadAsStringAsync();

					// 解析 XML 回應並反序列化為物件
					XmlSerializer responseSerializer = new XmlSerializer(typeof(T));
					using (var reader = new System.IO.StringReader(responseContent))
					{
						return (T)responseSerializer.Deserialize(reader);
					}
				}
				else
				{
					throw new HttpRequestException($"API call failed with status code {response.StatusCode}");
				}
			}
		}
	}


	[XmlRoot("ResponseModel")]
	public class ResponseModel
	{
		[XmlElement("Status")]
		public string Status { get; set; }

		[XmlElement("Message")]
		public string Message { get; set; }
	}


}
