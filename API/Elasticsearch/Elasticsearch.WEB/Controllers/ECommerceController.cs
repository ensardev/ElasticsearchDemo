using Elasticsearch.WEB.Services;
using Elasticsearch.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.WEB.Controllers
{
    public class ECommerceController : Controller
    {
        private readonly ECommerceService _eCommerceService;

        public ECommerceController(ECommerceService eCommerceService)
        {
            _eCommerceService = eCommerceService;
        }

        public async Task<IActionResult> Search([FromQuery] SearchPageViewModel searchPageViewModel)
        {
            var (list, totalCount, pageCount) = await _eCommerceService.SearchAsync(searchPageViewModel.SearchViewModel, searchPageViewModel.Page, searchPageViewModel.PageSize);

            searchPageViewModel.ECommerceList = list;
            searchPageViewModel.TotalCount = totalCount;
            searchPageViewModel.PageCount = pageCount;

            return View(searchPageViewModel);
        }
    }
}
