﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Praksa_projectV1.Models;

public partial class Job
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department Department { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}