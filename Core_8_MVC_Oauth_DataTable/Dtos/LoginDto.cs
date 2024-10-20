using System.ComponentModel.DataAnnotations;

namespace Core_8_MVC_Oauth_DataTable.Dtos
{
	public class LoginDto
	{
		[Required(ErrorMessage ="請填入{0}")]
		[Display(Name ="帳號")]
		public required string Account { get; set; }

		[Required(ErrorMessage = "請填入{0}")]
		[Display(Name = "密碼")]
		public required string Password { get; set; }
	}
}
