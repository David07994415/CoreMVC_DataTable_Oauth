using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
	public class UploadImageController : Controller
	{

		private readonly IWebHostEnvironment _environment;
		private readonly IConfiguration _configuration;

		public UploadImageController(IWebHostEnvironment environment, IConfiguration configuration)
		{
			_environment = environment;
			_configuration = configuration;
		}



		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public IActionResult Upload(UploadViewModel input)
		{
			if (input.files == null || input.files.Count == 0)
			{
				return BadRequest(new { message = "未選擇檔案" });
			}

			string[] allowedExtensions = { ".jpg", ".png", ".pdf" }; // 允許的副檔名
																	 // long maxFileSize = 5 * 1024 * 1024; // 最大大小 5 MB
																	 // 從 _configuration 讀取大小限制（以 MB 為單位）
			long maxFileSize = _configuration.GetValue<long>("FileUploadSettings:MaxFileSizeMB") * 1024 * 1024;

			string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
			// string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");

			var provider = new FileExtensionContentTypeProvider();


			if (!Directory.Exists(uploadFolder))
			{
				Directory.CreateDirectory(uploadFolder);
			}

			foreach (var file in input.files)
			{
				string extension = Path.GetExtension(file.FileName);

				// 檢查格式
				if (!allowedExtensions.Contains(extension.ToLower()))
				{
					return BadRequest(new { message = $"不允許的檔案格式：{extension}" });
				}
				// 檢查檔案magic number
				if (!IsImageFile(file))
				{
					return BadRequest(new { message = $"檔案 {file.FileName} 不是有效的圖片格式" });
				}

				if (!provider.TryGetContentType(file.FileName, out string contentType))
				{
					return BadRequest(new { message = $"無法解析的檔案類型：{file.FileName}" });
				}

				// 比對 ContentType 是否一致
				if (file.ContentType != contentType)
				{
					return BadRequest(new { message = $"檔案 {file.FileName} 的 ContentType 與副檔名不符" });
				}


				// 檢查大小
				if (file.Length > maxFileSize)
				{
					return BadRequest(new { message = $"檔案 {file.FileName} 超過大小限制 5 MB" });
				}

				// 重新命名檔案
				string newFileName = $"{Guid.NewGuid()}{extension}";
				string filePath = Path.Combine(uploadFolder, newFileName);

				// 儲存檔案
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					// await file.CopyToAsync(stream);
				}

				// 儲存檔案名稱到資料庫的邏輯 (假設你使用 Entity Framework)
				// Example:
				// var dbEntry = new FileRecord { FileName = newFileName, OriginalFileName = file.FileName };
				// _context.FileRecords.Add(dbEntry);
				// await _context.SaveChangesAsync();
			}

			return Ok(new { message = "檔案上傳成功" });

		}

		private bool IsImageFile(IFormFile file)
		{
			// 定義圖片格式的 Magic Number
			byte[][] imageHeaders = new byte[][]
			{
			new byte[] { 0xFF, 0xD8, 0xFF }, // JPEG
            new byte[] { 0x89, 0x50, 0x4E, 0x47 }, // PNG
            new byte[] { 0x47, 0x49, 0x46, 0x38 }, // GIF
            new byte[] { 0x42, 0x4D } // BMP
			};

			using (var stream = file.OpenReadStream())
			{
				byte[] headerBytes = new byte[8];
				stream.Read(headerBytes, 0, headerBytes.Length);

				foreach (var header in imageHeaders)
				{
					if (headerBytes.Take(header.Length).SequenceEqual(header))
					{
						return true;
					}
				}
			}

			return false;
		}



	}


	public class UploadViewModel
	{
        public string id { get; set; }
        public List<IFormFile> files { get; set; } = new List<IFormFile>();
	}


}
