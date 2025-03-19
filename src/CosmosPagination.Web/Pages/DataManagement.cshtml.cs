using CosmosPagination.Cosmos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosPagination.Web.Pages;

public class DataModel(IRepository repository) : PageModel
{
    public string Message { get; private set; } = string.Empty;

    private readonly IRepository _repository = repository;

    public void OnGet() { }
    public async Task OnPostSeedAsync()
    {
        await _repository.Seed();
        Message = "Succesfully seeded database";
    }

    public async Task OnPostDeleteAsync()
    {
        await _repository.Delete();
        Message = "Succesfully deleted database and container";
    }
}
