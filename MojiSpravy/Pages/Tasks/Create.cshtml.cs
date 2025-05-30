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
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public TaskItem Task { get; set; } = new();

    public SelectList Categories { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var categories = await _context.Categories
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.Name)
            .ToListAsync();

        Categories = new SelectList(categories, "Id", "Name");
        Task.DueDate = DateTime.Today.AddDays(1);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var categories = await _context.Categories
                    .Where(c => c.UserId == user.Id)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                Categories = new SelectList(categories, "Id", "Name");
            }
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }

        Task.UserId = currentUser.Id;
        Task.CreatedAt = DateTime.UtcNow;

        _context.Tasks.Add(Task);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
} 