using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MojiSpravy.Data;
using MojiSpravy.Models;

namespace MojiSpravy.Pages.Categories;

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
    public Category Category { get; set; } = default!;

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

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);

        if (category == null)
        {
            return NotFound();
        }

        Category = category;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var categoryToUpdate = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == Category.Id && c.UserId == user.Id);

        if (categoryToUpdate == null)
        {
            return NotFound();
        }

        categoryToUpdate.Name = Category.Name;
        categoryToUpdate.Description = Category.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(Category.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
} 