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
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public TaskItem Task { get; set; } = default!;

    public SelectList Categories { get; set; } = null!;

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
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

        if (task == null)
        {
            return NotFound();
        }

        Task = task;

        var categories = await _context.Categories
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.Name)
            .ToListAsync();

        Categories = new SelectList(categories, "Id", "Name", task.CategoryId);

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

                Categories = new SelectList(categories, "Id", "Name", Task.CategoryId);
            }
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }

        var taskToUpdate = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == Task.Id && t.UserId == currentUser.Id);

        if (taskToUpdate == null)
        {
            return NotFound();
        }

        taskToUpdate.Title = Task.Title;
        taskToUpdate.Description = Task.Description;
        taskToUpdate.DueDate = Task.DueDate;
        taskToUpdate.Status = Task.Status;
        taskToUpdate.CategoryId = Task.CategoryId;
        taskToUpdate.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(Task.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
} 