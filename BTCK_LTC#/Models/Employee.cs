using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BTCK_LTC_.Models;

public partial class Employee : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Birthday is required")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly? Birthday { get; set; }

    public string? Address { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required")]
    [RegularExpression(@"^(090|098|091|083|084|085|081|082|070|079|077|076|078|056|058|059|031|032|033|034|035|036|037|038|039)\d{7}$", ErrorMessage = "Invalid PhoneNumber")]
    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }

    public int? DerpartmentId { get; set; }

    public int? CompanyId { get; set; }

    public int? RoleId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Department? Derpartment { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role? Role { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Kiểm tra xem Username có trùng nhau không
        if (!string.IsNullOrEmpty(Username))
        {
            // Truy vấn cơ sở dữ liệu để kiểm tra xem Username đã tồn tại chưa
            var dbContext = (QuanLyBaiDangCongTyContext)validationContext.GetService(typeof(QuanLyBaiDangCongTyContext));
            var existingEmployee = dbContext.Employees.FirstOrDefault(e => e.Username == Username && e.Id != Id);

            if (existingEmployee != null)
            {
                yield return new ValidationResult("Username already exists", new[] { nameof(Username) });
            }
        }

        // Các điều kiện kiểm tra khác (nếu có)

        yield return ValidationResult.Success;
    }
}
