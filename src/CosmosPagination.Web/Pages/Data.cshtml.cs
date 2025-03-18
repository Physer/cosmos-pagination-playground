using CosmosPagination.Cosmos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosPagination.Web.Pages;

public class DataModel(IRepository repository) : PageModel
{
    private readonly IRepository _repository = repository;

    public void OnGet() { }
    public async Task OnPostSeedAsync() => await _repository.Seed();
    public async Task OnPostGetAllAsync() => await _repository.GetAll();
}
