using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core_8_MVC_Oauth_DataTable.Models
{
	[Table("Sms_Log")]
	public class SmsLog
	{
		[Key]
        public required string uid { get; set; }

		[Required]
		public required string pid { get; set; }

		[Required]
		public required DateTime SendTime { get; set; }

		[Required]
		public required string Code { get; set; }

		public bool? IsSendSuccess { get; set; }

		[MaxLength(200)]
		public string? ErrorMessage { get; set; }

		[Required]
		public required bool hasVerify { get; set; }

		public DateTime? VerifyTime { get; set; }
	}
}
