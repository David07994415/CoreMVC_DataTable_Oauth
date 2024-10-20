using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core_8_MVC_Oauth_DataTable.Models;

public partial class coredbContext : DbContext
{
    public coredbContext(DbContextOptions<coredbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<customer> customer { get; set; }

    public virtual DbSet<orderdetails> orderdetails { get; set; }

    public virtual DbSet<product> product { get; set; }

	public virtual DbSet<Logger> logger { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
