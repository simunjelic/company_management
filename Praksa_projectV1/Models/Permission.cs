﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Praksa_projectV1.Models;

public partial class Permission
{
    public int Id { get; set; }

    public int? ModuleId { get; set; }

    public int? RoleId { get; set; }

    public string Action { get; set; }

    public int? ActionId { get; set; }

    public virtual Module Module { get; set; }

    public virtual Role Role { get; set; }
}