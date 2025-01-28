using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Repositories;
using System.Net;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());

            if (response == null)
            {
                return ResponseDto<ProductDto>.Fail("An error occurred while saving the product.", HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productListDto = new List<ProductDto>();

            foreach (var product in products)
            {
                if (product.Feature is null)
                {
                    productListDto.Add(new ProductDto(product.Id, product.Name, product.Price, product.Stock, null));
                }
                else
                {
                    productListDto.Add(new ProductDto(product.Id, product.Name, product.Price, product.Stock,
                        new ProductFeatureDto(product.Feature.Width, product.Feature.Height, product.Feature.Color.ToString())));
                }
            }

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct = await _productRepository.GetByIdAsync(id);

            if (hasProduct == null)
            {
                return ResponseDto<ProductDto>.Fail("Product not found.", HttpStatusCode.NotFound);
            }

            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var isSuccessful = await _productRepository.UpdateAsync(updateProduct);

            if (!isSuccessful)
            {
                return ResponseDto<bool>.Fail("An error occurred while updating the product.", HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(isSuccessful, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var deleteResponse = await _productRepository.DeleteAsync(id);

            if(!deleteResponse.IsValidResponse && deleteResponse.Result == Result.NotFound)
            {
                return ResponseDto<bool>.Fail("Product not found.", HttpStatusCode.NotFound);
            }

            if (!deleteResponse.IsValidResponse)
            {
                deleteResponse.TryGetOriginalException(out Exception? originalException);
                _logger.LogError(originalException, deleteResponse.ElasticsearchServerError?.Error.ToString());

                return ResponseDto<bool>.Fail("An error occurred while deleting the product.", HttpStatusCode.InternalServerError);
            }


            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

    }
}
