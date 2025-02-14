﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AlarmSystem.Models;

public partial class Js_LineAlarmContext : DbContext
{
    public Js_LineAlarmContext(DbContextOptions<Js_LineAlarmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlarmGroup> AlarmGroup { get; set; }

    public virtual DbSet<AlarmItem> AlarmItem { get; set; }

    public virtual DbSet<AlarmSettings> AlarmSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlarmGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK_LineGroups");

            entity.Property(e => e.GroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Enable).HasDefaultValue(true);
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .HasDefaultValue("尚未命名");
        });

        modelBuilder.Entity<AlarmItem>(entity =>
        {
            entity.HasKey(e => e.Stid).HasName("PK_AlarmItems");

            entity.Property(e => e.Stid)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DelayTime).HasDefaultValue(60);
            entity.Property(e => e.Enable).HasDefaultValue(true);
            entity.Property(e => e.GroupId)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(30);
        });

        modelBuilder.Entity<AlarmSettings>(entity =>
        {
            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.NextCheckTime).HasColumnType("datetime");
            entity.Property(e => e.ParameterColumn)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ParameterShow)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(e => e.StartTime).HasPrecision(0);
            entity.Property(e => e.Stid)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}