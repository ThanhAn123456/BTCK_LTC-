using System;
using System.Collections.Generic;

namespace BTCK_LTC_.Models;

public partial class Post
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Thumbnail { get; set; }

    public DateOnly? Date { get; set; }

    public DateOnly? PostDate { get; set; }

    public int? EmployeeId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Employee? Employee { get; set; }
}
