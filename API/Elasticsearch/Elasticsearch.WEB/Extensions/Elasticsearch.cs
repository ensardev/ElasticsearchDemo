using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Elasticsearch.WEB.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            var userName = configuration.GetSection("Elastic")["Username"];
            var password = configuration.GetSection("Elastic")["Password"];
            var elasticUrl = configuration.GetSection("Elastic")["Url"];

            var settings = new ElasticsearchClientSettings(new Uri(elasticUrl!)).Authentication(new BasicAuthentication(userName!, password!));
            var client = new ElasticsearchClient(settings);

            services.AddSingleton(client);
        }
    }
}
