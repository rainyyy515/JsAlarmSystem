﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AlarmSystem.Models;

public partial class AlarmItem
{
    public string Stid { get; set; }

    public string GroupId { get; set; }

    public string Location { get; set; }

    public int DelayTime { get; set; }

    public bool Enable { get; set; }

    public bool BreakAlarm { get; set; }
}