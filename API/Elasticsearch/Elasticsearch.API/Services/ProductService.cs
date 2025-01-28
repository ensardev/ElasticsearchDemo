using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;
using Elasticsearch.API.Repositories;
using System.Collections.Immutable;
using System.Net;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var response = await _productRepository.SaveAsync(request.CreateProduct());

            if (response == null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "An error occurred while saving the product." }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<ProductDto>.Success(response.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productListDto = new List<ProductDto>();

            foreach (var product in products)
            {
                if(product.Feature is null)
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

            if(hasProduct == null)
            {
                return ResponseDto<ProductDto>.Fail(new List<string> { "Product not found." }, HttpStatusCode.NotFound);
            }

            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);
        }
    }
}
