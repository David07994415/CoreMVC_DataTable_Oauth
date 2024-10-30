using System.ComponentModel.DataAnnotations;

namespace Core_8_MVC_Oauth_DataTable.Dtos
{
	public class SmsApiDto
	{
	}

	public class SmsApiRequestDto
	{
		[Display(Name = "交易代號")]
		[Required] // 機關自訂交易編號
		[MaxLength(36)]
		public required string orderNo { get; set; }

		[Display(Name = "交易批號")]
		[Required] // 機關交易編號的批號
		[MaxLength(5)]
		public required string packageNo { get; set; }

		[Display(Name = "這批共有幾筆")]
		[Required] // 檢查是否全數字(1~500)
		[MaxLength(4)]
		public required string packageCnt { get; set; }

		public List<SmsApiRequestBodyDto> smsBody { get; set; } = new List<SmsApiRequestBodyDto>();

    }

	public class SmsApiRequestBodyDto
	{
		[Display(Name = "識別碼")]
		[Required] // 單位自定義
		[MaxLength(20)]
		public required string uid { get; set; }

		[Display(Name = "手機號碼")]
		[Required] // 檢查長度及是否皆為數字，格式為09xxxxxxxx
		[MaxLength(10)]
		public required string mobile { get; set; }

		[Display(Name = "簡訊內容")]
		[Required] // 英數字160 byte，中文字70字
		[MaxLength(70)]
		public required string smsText { get; set; }
	}

	public class SmsApiResponseDto
	{
		[Display(Name = "這批簡訊識別碼")]
		[Required] // 簡訊平臺的唯一識別碼（這批交易）
		[MaxLength(19)]
		public string msgId { get; set; }

		[Display(Name = "狀態")]
		[Required] // 1.回應正常為0000 2.非0000則為錯誤
		[MaxLength(4)]
		public required string status { get; set; }

		[Display(Name = "代碼訊息")]
		[Required] // status的文字
		[MaxLength(100)]
		public string statusMsg { get; set; }

		[Display(Name = "交易代號")]
		// 機關自訂交易編號
		[MaxLength(36)]
		public  string orderNo { get; set; }

		[Display(Name = "交易批號")]
		 // 機關交易編號的批號
		[MaxLength(5)]
		public  string packageNo { get; set; }

		[Display(Name = "這批共有幾筆")]
		[Required]   // 平台實際處理的數量
		[MaxLength(4)]
		public required string packageCnt { get; set; }

		[Display(Name = "接收失敗清單")] // 接收失敗筆數>0才有
		public List<SmsApiResponseErrorDto> errorUids { get; set; } = new List<SmsApiResponseErrorDto>();
	}

	public class SmsApiResponseErrorDto 
	{
		public string uid{ get; set; }
		public string uidStatus { get; set; }
		public string uidStatusMsg { get; set; }
	}

}

