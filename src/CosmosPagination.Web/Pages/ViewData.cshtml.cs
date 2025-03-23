using CosmosPagination.Cosmos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace CosmosPagination.Web.Pages;

public class ViewDataModel(IRepository repository, IMemoryCache memoryCache) : PageModel
{
    public IEnumerable<Product> Products { get; private set; } = [];
    public long PageCount { get; private set; } = 1;

    private readonly IRepository _repository = repository;
    private readonly IMemoryCache _memoryCache = memoryCache;

    private const int _pageSize = 25;
    private const string _pageCountKey = "PageCount";

    public async Task OnGetAsync()
    {
        var itemCount = await _repository.GetItemCount();
        var pageCount = itemCount / _pageSize;
        _memoryCache.Set(_pageCountKey, pageCount);
    }

    public async Task OnPostLoadUnpaginatedAsync() => Products = await _repository.GetAll();
    public async Task OnPostLoadSkipTake(int pageNumber)
    {
        PageCount = _memoryCache.Get<long>(_pageCountKey);
        Products = await _repository.GetPaginatedResults(pageNumber, _pageSize);
    }
}
