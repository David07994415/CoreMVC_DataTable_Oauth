using Core_8_MVC_Oauth_DataTable.Dtos;
using Core_8_MVC_Oauth_DataTable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
    public class DataTableController : Controller
	{
		private readonly coredbContext _db;

		public DataTableController(coredbContext db)
        {
			_db = db;
		}
        public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public IActionResult GetTableData([FromForm] DataTableRequest request)
		{

			// 查詢數據的初步集合
			var query = _db.logger.AsQueryable();

			// 處理全局搜尋
			if (!string.IsNullOrEmpty(request.Search?.Value))
			{
				var searchValue = request.Search.Value;
				query = query.Where(x => x.UserName.Contains(searchValue) || x.UrlPath.Contains(searchValue));
			}

			// 計算總記錄數（未過濾）==> 如果關閉 JS DataTable 的 Search 功能，totalRecords 可以等於 filteredRecords
			// var totalRecords = query.Count();
			// PS：數據量過大，考慮關閉

			// 處理排序
			if (request.Order != null && request.Order.Length > 0)
			{
				var columnIndex = request.Order[0].Column; // 排序的列索引
				var sortDirection = request.Order[0].Dir; // 排序方向 asc 或 desc

				// 根據列索引進行排序
				switch (columnIndex)
				{
					case 0:
						query = sortDirection == "asc" ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
						break;
					case 1:
						query = sortDirection == "asc" ? query.OrderBy(x => x.UserName) : query.OrderByDescending(x => x.UserName);
						break;
					case 2:
						query = sortDirection == "asc" ? query.OrderBy(x => x.UrlPath) : query.OrderByDescending(x => x.UrlPath);
						break;
					case 3:
						query = sortDirection == "asc" ? query.OrderBy(x => x.LogTime) : query.OrderByDescending(x => x.LogTime);
						break;
				}
			}

			// 新增：針對使用者 Input 進行 Where 搜尋
			if (!string.IsNullOrEmpty(request.UserName))
			{
				query = query.Where(x => x.UserName.Contains(request.UserName));
			}
			if (!string.IsNullOrEmpty(request.UrlPath))
			{
				query = query.Where(x => x.UrlPath.Contains(request.UrlPath));
			}


			// 處理分頁
			var filteredRecords = query.Count();
			var data = query
				.Skip(request.Start)      // 跳過記錄數
				.Take(request.Length)  // 取得記錄數
				.Select(x => new
				{
					x.Id,
					x.UserName,
					x.UrlPath,
					x.LogTime
				})
				.ToList();

			// 準備 Response Data
			var response = new DataTableResponse
			{
				Draw = request.Draw,
				// RecordsTotal = totalRecords,       // 總記錄數，PS：數據量過大，考慮關閉
				RecordsFiltered = filteredRecords, // 過濾後的記錄數
				Data = data                        // 返回的數據
			};

			return Ok(response);

		}

	}
}
