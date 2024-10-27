using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

public partial class MemberShip
{
    [Key]
    public int SN { get; set; }

    [StringLength(50)]
    public string Phone { get; set; } = null!;

    [StringLength(50)]
    public string Pid { get; set; } = null!;
}
