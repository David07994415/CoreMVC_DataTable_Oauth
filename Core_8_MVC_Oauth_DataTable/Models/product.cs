using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

public partial class product
{
    [Key]
    public int Ids { get; set; }

    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(6, 2)")]
    public decimal Price { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<orderdetails> orderdetails { get; set; } = new List<orderdetails>();
}
