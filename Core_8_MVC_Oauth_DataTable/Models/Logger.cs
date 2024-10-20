using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core_8_MVC_Oauth_DataTable.Models
{
	[Table("Logger")]
	public class Logger
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		[MaxLength(50)]
		public string UserName { get; set; } = null!;

		[MaxLength(50)]
		public string UrlPath { get; set; } = null!;


		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime LogTime { get; set; }

	}
}
