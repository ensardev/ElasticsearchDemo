using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.WEB.Models;

namespace Elasticsearch.WEB.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "blog";

        public BlogRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _client.IndexAsync(newBlog, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsValidResponse) return null;

            newBlog.Id = response.Id;
            return newBlog;
        }

        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            List<Action<QueryDescriptor<Blog>>> listQuery = new();

            Action<QueryDescriptor<Blog>> matchAll = q => q.MatchAll(m => { });

            Action<QueryDescriptor<Blog>> matchContent = q => q.Match(m => m.Field(f => f.Content).Query(searchText));

            Action<QueryDescriptor<Blog>> matchTitle = q => q.MatchBoolPrefix(m => m.Field(f => f.Title).Query(searchText));

            if (string.IsNullOrEmpty(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(matchTitle);
            }

            //Search by Title and Content
            var searchResponse = await _client.SearchAsync<Blog>(x => x.Index(indexName).Query(q => q.Bool(b => b.Should(listQuery.ToArray()))));

            foreach (var hit in searchResponse.Hits) hit.Source.Id = hit.Id;

            return searchResponse.Documents.ToList();
        }
    }
}
