using Elasticsearch.WEB.Repositories;
using Elasticsearch.WEB.ViewModels;

namespace Elasticsearch.WEB.Services
{
    public class ECommerceService
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceService(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        public async Task<(List<ECommerceViewModel>, long Count, int pageCount)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            var (list, totalCount) = await _eCommerceRepository.SearchAsync(searchViewModel, page, pageSize);
            var pageLinkCount = (int)Math.Ceiling((double)totalCount / pageSize);

            var eCommerceList = list.Select(x => new ECommerceViewModel
            {
                Id = x.Id,
                CustomerFirstName = x.CustomerFirstName,
                CustomerLastName = x.CustomerLastName,
                CustomerFullName = x.CustomerFullName,
                Gender = x.Gender,
                OrderId = x.OrderId,
                OrderDate = x.OrderDate.ToString("yyyy-MM-dd"),
                TaxfulTotalPrice = x.TaxfulTotalPrice,
                Category = String.Join(",", x.Category)
            });

            return (eCommerceList.ToList(), totalCount, pageLinkCount);
        }
    }
}
