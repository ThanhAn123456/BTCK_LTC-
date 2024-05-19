using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTCK_LTC_.Models;

public partial class Post
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(150, MinimumLength = 20, ErrorMessage = "Title must be between 20 and 150 characters.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public string? Content { get; set; }

    public string? Thumbnail { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly? Date { get; set; }

    public DateOnly? PostDate { get; set; }

    public int? EmployeeId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Employee? Employee { get; set; }
}
