using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MojiSpravy.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва категорії обов'язкова")]
    [StringLength(100, ErrorMessage = "Назва категорії не може перевищувати 100 символів")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Опис не може перевищувати 500 символів")]
    public string? Description { get; set; }

    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
} 