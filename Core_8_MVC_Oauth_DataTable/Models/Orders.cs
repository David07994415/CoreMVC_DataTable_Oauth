using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

[Index("CustomerID", Name = "IX_Orders_CustomerID")]
public partial class Orders
{
    [Key]
    public int Ids { get; set; }

    public DateTime OrderPlaced { get; set; }

    public DateTime OrderFulfilled { get; set; }

    public int CustomerID { get; set; }

    [ForeignKey("CustomerID")]
    [InverseProperty("Orders")]
    public virtual customer Customer { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<orderdetails> orderdetails { get; set; } = new List<orderdetails>();
}
