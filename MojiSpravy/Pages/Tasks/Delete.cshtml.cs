using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MojiSpravy.Data;
using MojiSpravy.Models;

namespace MojiSpravy.Pages.Tasks;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public TaskItem Task { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

        if (task == null)
        {
            return NotFound();
        }

        Task = task;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

        if (task == null)
        {
            return NotFound();
        }

        Task = task;
        _context.Tasks.Remove(Task);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
} 