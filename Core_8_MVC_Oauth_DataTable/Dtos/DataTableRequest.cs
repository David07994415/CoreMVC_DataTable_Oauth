namespace Core_8_MVC_Oauth_DataTable.Dtos
{
    public class DataTableRequest
    {
		// 參數參考網址：https://datatables.net/manual/server-side
		public int Draw { get; set; }          // 繪製次數
		public int Start { get; set; }         // 當前頁的第一條記錄的索引
		public int Length { get; set; }        // 每頁顯示的記錄數量
		public DataTableSearch Search { get; set; }   // 搜尋參數
		public DataTableOrder[] Order { get; set; }   // 排序參數


		// 新增：使用者傳入的參數
		public string UserName { get; set; }   // 過濾用戶名
		public string UrlPath { get; set; }       // 過濾日期
	}
	public class DataTableSearch
	{
		public string Value { get; set; }       // 搜尋字串
	}

	public class DataTableOrder
	{
		public int Column { get; set; }         // 排序的列索引
		public string Dir { get; set; }         // 排序方向 asc 或 desc
	}

}
