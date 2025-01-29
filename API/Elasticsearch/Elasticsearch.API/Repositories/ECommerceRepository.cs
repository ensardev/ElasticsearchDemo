using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModels;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        private const string indexName = "kibana_sample_data_ecommerce";

        public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
        {

            #region Type Unsafe Example
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //    s.Index(indexName)
            //    .Query(q =>
            //        q.Term(t =>
            //            t.Field("customer_first_name.keyword")
            //            .Value(customerFirstName)
            //        )
            //    )
            //);
            #endregion

            #region Type Safety Example
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //    s.Index(indexName)
            //    .Query(q =>
            //        q.Term(t =>
            //            t.Field(f=>
            //                f.CustomerFirstName.Suffix("keyword"))
            //                .Value(customerFirstName)
            //        )
            //    )
            //);
            #endregion

            #region Other Way
            var termQuery = new TermQuery("cutomer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true,
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));
            #endregion


            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();

        }
    }
}
