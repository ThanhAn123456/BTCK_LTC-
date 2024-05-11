using System;
using System.Collections.Generic;

namespace BTCK_LTC_.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? DerpartmentId { get; set; }

    public int? CompanyId { get; set; }

    public int? RoleId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Department? Derpartment { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role? Role { get; set; }
}
