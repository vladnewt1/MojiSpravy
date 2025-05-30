using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MojiSpravy.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва завдання обов'язкова")]
    [StringLength(200, ErrorMessage = "Назва завдання не може перевищувати 200 символів")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Опис не може перевищувати 1000 символів")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Дата виконання обов'язкова")]
    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    Cancelled
} 