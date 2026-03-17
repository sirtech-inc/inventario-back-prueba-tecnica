using Application.Dtos.Product;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<Results<BadRequest, Ok<PaginadoResponse<ProductDto>>>> BusquedaPaginado([FromQuery] PaginationRequest dto)
        {
            var response = await _productService.BusquedaPaginado(dto);

            if (response != null)
            {
                return TypedResults.Ok(response);
            }

            return TypedResults.BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<Results<BadRequest, Ok<ProductDto>>> Get(string id)
        {
            try
            {

                var response = await _productService.FindByIdAsync(id);

                return response != null
                    ? TypedResults.Ok(response)
                    : TypedResults.BadRequest();
            }
            catch
            {
                return TypedResults.BadRequest();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Results<BadRequest, Ok<OperationResult<ProductDto>>>> Post([FromBody] ProductRequestDto request)
        {
            var response = await _productService.CreateAsync(request);

            if (response != null) return TypedResults.Ok(response);

            return TypedResults.BadRequest();
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<Results<BadRequest, Ok<OperationResult<ProductDto>>>> Put(string id, [FromBody] ProductRequestDto request)
        {
            try
            {

                var response = await _productService.EditAsync(id, request);

                return response != null
                    ? TypedResults.Ok(response)
                    : TypedResults.BadRequest();
            }
            catch
            {
                return TypedResults.BadRequest();
            }
        }
    }
}
