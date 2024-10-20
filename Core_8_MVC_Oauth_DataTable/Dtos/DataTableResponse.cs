namespace Core_8_MVC_Oauth_DataTable.Dtos
{
	public class DataTableResponse
	{
		// 參數參考網址：https://datatables.net/manual/server-side
		public int Draw { get; set; }          // 繪製次數
		public int RecordsTotal { get; set; }        // 總記錄數
		public int RecordsFiltered { get; set; }     // 過濾後的記錄數
		public object Data { get; set; }     // 返回的數據
		public string Error { get; set; }   // 錯誤訊息
	}
}
