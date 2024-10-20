using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

[Index("OrderID", Name = "IX_orderdetails_OrderID")]
[Index("ProductID", Name = "IX_orderdetails_ProductID")]
public partial class orderdetails
{
    [Key]
    public int Ids { get; set; }

    public int Quantity { get; set; }

    public int ProductID { get; set; }

    public int OrderID { get; set; }

    [ForeignKey("OrderID")]
    [InverseProperty("orderdetails")]
    public virtual Orders Order { get; set; } = null!;

    [ForeignKey("ProductID")]
    [InverseProperty("orderdetails")]
    public virtual product Product { get; set; } = null!;
}
