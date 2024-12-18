﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace handover_api.Models;

[Table("handover_sheet_category_setting")]
public partial class HandoverSheetCategorySetting
{
    [Key]
    [StringLength(100)]
    public string CategoryId { get; set; } = null!;

    [Column("MainSheetID")]
    public int? MainSheetId { get; set; }

    [Column("SheetGroupID")]
    public int? SheetGroupId { get; set; }

    [StringLength(100)]
    public string? WeekDays { get; set; }

    [StringLength(200)]
    public string CategoryName { get; set; } = null!;

    [Column(TypeName = "timestamp")]
    public DateTime? CreatedTime { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? UpdatedTime { get; set; }

    public int? CategoryRank { get; set; }
}