namespace Elasticsearch.WEB.ViewModels
{
    public class SearchPageViewModel
    {
        public long TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int PageCount { get; set; }
        public List<ECommerceViewModel> ECommerceList { get; set; }
        public ECommerceSearchViewModel SearchViewModel { get; set; }
        
        
        public int StartPage()
        {
            int start = Page - 4;
            return Math.Max(1, start);
        }
        public int EndPage()
        {
            int start = StartPage();
            int end = start + 10;
            return Math.Min(PageCount, end);
        }

        public string CreatePageUrl(HttpRequest request, int page, int pageSize)
        {
            var currentUrl = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").AbsoluteUri;

            if (currentUrl.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                currentUrl = currentUrl.Replace($"Page={Page}", $"Page={page}", StringComparison.OrdinalIgnoreCase);
                currentUrl = currentUrl.Replace($"PageSize={PageSize}", $"PageSize={pageSize}", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                string separator = currentUrl.Contains("?") ? "&" : "?";
                currentUrl += $"{separator}Page={page}&PageSize={pageSize}";
            }
            return currentUrl;
        }

    }
}
