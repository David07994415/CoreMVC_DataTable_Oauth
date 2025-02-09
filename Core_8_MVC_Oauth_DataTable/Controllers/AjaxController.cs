using Core_8_MVC_Oauth_DataTable.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class AjaxController : Controller
	{
		public IActionResult Index()
		{
			AjaxIndexRenderViewModel model = new();
			model.AreaList.Insert(0,new SelectListItem(text: "請選擇", value: ""));



			return View(model);
		}

		[HttpPost]
		public IActionResult GetDivList(
			[Required(ErrorMessage ="沒有選擇行政區")]
			[MaxLength(2,ErrorMessage ="超過兩個字")]
			string AreaCode)
		{
			ResultDtos resultDtos_Fail = new("Error", "");
			ResultDtos resultDtos_Success = new("Success", "");

			if (!ModelState.IsValid)
			{
				// 收集所有驗證錯誤訊息
				var errorMessages = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				// 錯誤處理
				resultDtos_Fail.Message = string.Join("; ", errorMessages);
				return Ok(resultDtos_Fail);
			}

			var DivDataSet = new AjaxIndexRenderFakeDivModelList().DivList
				.Where(x => x.AreaCode == AreaCode).Select(x=>new SelectListItem() { 
					Text = x.DivName,
					Value = x.DivCode
				}).ToList();

			//List<SelectListItem> ReturnDivList = new();
			//foreach (var DivData in DivDataSet)
			//{
			//	SelectListItem temp = new();
			//	temp.Text = DivData.DivName;
			//	temp.Value = DivData.DivCode;
			//	ReturnDivList.Add(temp);
			//}

			resultDtos_Success.Data = DivDataSet;
			return Ok(resultDtos_Success);
		}
	}


	public class AjaxIndexRenderViewModel
	{
		public List<SelectListItem> AreaList { get; set; } =
			[ 
			  new (text: "鼓山區", value: "01")
			, new (text: "左營區", value: "02")
			];
	}


	public class AjaxIndexRenderFakeDivModel
	{
		public string AreaCode { get; set; } = null!;
		public string DivCode { get; set; } = null!;
		public string DivName { get; set; } = null!;
	}

	public class AjaxIndexRenderFakeDivModelList
	{
		public List<AjaxIndexRenderFakeDivModel> DivList { get; set; } = new() {
			new AjaxIndexRenderFakeDivModel { AreaCode = "01", DivCode = "01", DivName = "鼓山區01里" },
			new AjaxIndexRenderFakeDivModel { AreaCode = "01", DivCode = "02", DivName = "鼓山區02里" },
			new AjaxIndexRenderFakeDivModel { AreaCode = "02", DivCode = "01", DivName = "左營區01里" },
			new AjaxIndexRenderFakeDivModel { AreaCode = "02", DivCode = "02", DivName = "左營區02里" },
		};
	}
}
