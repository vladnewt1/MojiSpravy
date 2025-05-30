using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MojiSpravy.Data;
using MojiSpravy.Models;

namespace MojiSpravy.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<IndexModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskItem> RecentTasks { get; set; } = new();
    public List<TaskItem> UpcomingTasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        var tasks = await _context.Tasks
            .Include(t => t.Category)
            .Where(t => t.UserId == user.Id)
            .ToListAsync();

        var today = DateTime.Today;

        TotalTasks = tasks.Count;
        CompletedTasks = tasks.Count(t => t.Status == MojiSpravy.Models.TaskStatus.Completed);
        PendingTasks = tasks.Count(t => t.Status != MojiSpravy.Models.TaskStatus.Completed && t.Status != MojiSpravy.Models.TaskStatus.Cancelled);
        OverdueTasks = tasks.Count(t => t.Status != MojiSpravy.Models.TaskStatus.Completed && 
                                       t.Status != MojiSpravy.Models.TaskStatus.Cancelled && 
                                       t.DueDate < today);

        RecentTasks = tasks
            .Where(t => t.Status != MojiSpravy.Models.TaskStatus.Completed && t.Status != MojiSpravy.Models.TaskStatus.Cancelled)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToList();

        UpcomingTasks = tasks
            .Where(t => t.Status != MojiSpravy.Models.TaskStatus.Completed && 
                       t.Status != MojiSpravy.Models.TaskStatus.Cancelled && 
                       t.DueDate >= today)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .ToList();
    }
}
