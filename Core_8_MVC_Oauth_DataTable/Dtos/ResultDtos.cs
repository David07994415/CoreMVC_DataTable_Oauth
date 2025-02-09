namespace Core_8_MVC_Oauth_DataTable.Dtos
{
	public class ResultDtos(string status, string message)
	{
		public  string Status { get; set; } = status;

		public  string Message { get; set; } = message;

		public object? Data { get; set; }
	}
}
