﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Praksa_projectV1.Models;

public partial class EmployeeProject
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? EmployeeId { get; set; }

    public string Manager { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Project Project { get; set; }
}