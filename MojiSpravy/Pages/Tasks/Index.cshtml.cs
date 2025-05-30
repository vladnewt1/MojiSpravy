using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojiSpravy.Data;
using MojiSpravy.Models;

namespace MojiSpravy.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public SelectList Categories { get; set; } = null!;
    public int? SelectedCategoryId { get; set; }
    public Models.TaskStatus? SelectedStatus { get; set; }

    public async Task OnGetAsync(int? SelectedCategoryId, string? SelectedStatus)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        this.SelectedCategoryId = SelectedCategoryId;

        Models.TaskStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(SelectedStatus) && Enum.TryParse(SelectedStatus, out Models.TaskStatus s))
            parsedStatus = s;
        this.SelectedStatus = parsedStatus;

        var categories = await _context.Categories
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.Name)
            .ToListAsync();

        Categories = new SelectList(categories, "Id", "Name", SelectedCategoryId);

        var query = _context.Tasks
            .Include(t => t.Category)
            .Where(t => t.UserId == user.Id);

        if (SelectedCategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == SelectedCategoryId);
        }

        if (parsedStatus.HasValue)
        {
            query = query.Where(t => t.Status == parsedStatus);
        }

        Tasks = await query
            .OrderByDescending(t => t.DueDate)
            .ToListAsync();
    }
} 