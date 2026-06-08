using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ButceTakipSistemi.Services;
using ButceTakipSistemi.ViewModels;

namespace ButceTakipSistemi.Pages.Categories;

public class IndexModel(ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public CategoryInputModel Input { get; set; } = new();

    public IReadOnlyList<CategoryListItemViewModel> Categories { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var result = await categoryService.CreateAsync(Input);
        if (!result.Success)
        {
            ModelState.AddModelError(nameof(Input.Name), result.Error ?? "Kategori eklenemedi.");
            await LoadAsync();
            return Page();
        }

        TempData["SuccessMessage"] = "Kategori eklendi.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var result = await categoryService.ToggleActiveAsync(id);
        TempData[result.Success ? "SuccessMessage" : "WarningMessage"] = result.Message;
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Categories = await categoryService.GetCategoriesAsync();
    }
}
