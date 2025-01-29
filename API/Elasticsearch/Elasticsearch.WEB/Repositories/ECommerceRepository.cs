using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.ViewModels;

namespace Elasticsearch.WEB.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }


        public async Task<(List<ECommerce>, long Count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            if (!string.IsNullOrEmpty(searchViewModel.Category))
            {
                Action<QueryDescriptor<ECommerce>> query = q => q.Match(m => m.Field(f => f.Category).Query(searchViewModel.Category));
                listQuery.Add(query);
            }

            if (!string.IsNullOrEmpty(searchViewModel.CustomerFullName))
            {
                Action<QueryDescriptor<ECommerce>> query = q => q.Match(m => m.Field(f => f.CustomerFullName).Query(searchViewModel.CustomerFullName));
                listQuery.Add(query);
            }

            if (searchViewModel.OrderDateStart.HasValue)
            {
                Action<QueryDescriptor<ECommerce>> query = q => q.Range(r => r.DateRange(d => d.Field(f => f.OrderDate).Gte(searchViewModel.OrderDateStart.Value)));
                listQuery.Add(query);
            }

            if (searchViewModel.OrderDateEnd.HasValue)
            {
                Action<QueryDescriptor<ECommerce>> query = q => q.Range(r => r.DateRange(d => d.Field(f => f.OrderDate).Lte(searchViewModel.OrderDateEnd.Value)));
                listQuery.Add(query);
            }

            if (!string.IsNullOrEmpty(searchViewModel.Gender))
            {
                Action<QueryDescriptor<ECommerce>> query = q => q.Term(t => t.Field(f => f.CustomerFullName).Value(searchViewModel.Gender));
            }

            var pageFrom = (page - 1) * pageSize;

            var result = await _client.SearchAsync<ECommerce>(x => x.Index(indexName).Query(q => q.Bool(b => b.Must(listQuery.ToArray()))).From(pageFrom).Size(pageSize));

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return (result.Documents.ToList(), result.Total);
        }

    }
}
