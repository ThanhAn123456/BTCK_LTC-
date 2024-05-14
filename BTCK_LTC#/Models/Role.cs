using System;
using System.Collections.Generic;

namespace BTCK_LTC_.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? Post { get; set; }

    public bool? Manage { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
