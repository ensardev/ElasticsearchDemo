using Elasticsearch.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceController(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            var result = await _eCommerceRepository.TermQueryAsync(customerFirstName);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNameList)
        {
            var result = await _eCommerceRepository.TermsQueryAsync(customerFirstNameList);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            var result = await _eCommerceRepository.PrefixQueryAsync(customerFullName);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            var result = await _eCommerceRepository.RangeQueryAsync(fromPrice, toPrice);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            var result = await _eCommerceRepository.MatchAllQueryAsync();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQueryWithPagination(int page, int pageSize)
        {
            var result = await _eCommerceRepository.MatchAllQueryWithPaginationAsync(page, pageSize);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            var result = await _eCommerceRepository.WildCardQueryAsync(customerFullName);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerFullName)
        {
            var result = await _eCommerceRepository.FuzzyQueryAsync(customerFullName);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> MatchQueryFullText(string categoryName)
        {
            var result = await _eCommerceRepository.MatchQueryFullTextAsync(categoryName);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixFullText(string customerFullName)
        {
            var result = await _eCommerceRepository.MatchBoolPrefixQueryFullTextAsync(customerFullName);

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> MatchPhraseFullText(string customerFullName)
        {
            var result = await _eCommerceRepository.MatchPhraseQueryFullTextAsync(customerFullName);

            return Ok(result);
        }
    }
}
