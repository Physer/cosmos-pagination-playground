using CosmosPagination.Cosmos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosPagination.Web.Pages;

public class ViewDataModel(IRepository repository) : PageModel
{
    public IEnumerable<Product> Products { get; private set; } = [];

    private readonly IRepository _repository = repository;

    public void OnGet() { }

    public async Task OnPostLoadUnpaginatedAsync() => Products = await _repository.GetAll();
}
