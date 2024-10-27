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

    public virtual DbSet<Logger> Logger { get; set; }

    public virtual DbSet<MemberShip> MemberShip { get; set; }

    public virtual DbSet<OAuthTable> OAuthTable { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<customer> customer { get; set; }

    public virtual DbSet<orderdetails> orderdetails { get; set; }

    public virtual DbSet<product> product { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Logger>(entity =>
        {
            entity.Property(e => e.LogTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<OAuthTable>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
