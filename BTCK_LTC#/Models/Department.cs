﻿using System;
using System.Collections.Generic;

namespace BTCK_LTC_.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
