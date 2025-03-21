﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AlarmSystem.Models;

public partial class AlarmSettings
{
    public int Id { get; set; }

    public string Stid { get; set; }

    [DisplayName("API Parameter")]
    public string ParameterColumn { get; set; }

    [DisplayName("欄位名稱")]
    public string ParameterShow { get; set; }

    [DisplayName("閾值")]
    public int Threshold { get; set; }

    [DisplayName("開始時間")]
    public TimeOnly StartTime { get; set; }

    [DisplayName("結束時間")]
    public TimeOnly EndTime { get; set; }

    public DateTime? NextCheckTime { get; set; }
}