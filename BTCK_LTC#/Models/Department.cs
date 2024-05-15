using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTCK_LTC_.Models;

public partial class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
