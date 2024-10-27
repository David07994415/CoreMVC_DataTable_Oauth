using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

public partial class OAuthTable
{
    [Key]
    public int Sn { get; set; }

    public int MemberShIpSn { get; set; }

    public bool IsPhoneVerify { get; set; }

    [StringLength(100)]
    public string? GoogleId { get; set; }

    public bool? RebindGoogle { get; set; }

    [StringLength(100)]
    public string? LineId { get; set; }

    public bool? RebindLine { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LoginDate { get; set; }

    public bool IsExpired { get; set; }
}
