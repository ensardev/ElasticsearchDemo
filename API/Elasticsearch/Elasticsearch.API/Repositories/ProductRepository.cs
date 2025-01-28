using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;
using Nest;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string productsIndexName = "products";

        public ProductRepository(ElasticClient client)
        {
            _client = client;
        }

        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created = DateTime.Now;

            var response = await _client.IndexAsync(newProduct, x => x.Index(productsIndexName).Id(Guid.NewGuid().ToString()));

            if (!response.IsValid) return null;

            newProduct.Id = response.Id;
            return newProduct;
        }

        public async Task<ImmutableList<Product>> GetAllAsync()
        {
            var response = await _client.SearchAsync<Product>(s => s.Index(productsIndexName).Query(q => q.MatchAll()));

            foreach (var hit in response.Hits) hit.Source.Id = hit.Id;

            return response.Documents.ToImmutableList();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var response = await _client.GetAsync<Product>(id, x => x.Index(productsIndexName));

            if (!response.IsValid)
                return null;

            response.Source.Id = response.Id;
            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(updateProduct.Id, x => x.Index(productsIndexName).Doc(updateProduct));

            return response.IsValid;
        }

        /// <summary>
        /// This method is discussed for error management.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x => x.Index(productsIndexName));

            return response;
        }
    }
}
