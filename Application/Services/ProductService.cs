using Application.Dtos.Product;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entities;
using Infraestructure.Persistences.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(
            IMapper mapper,
            IProductRepository productRepositorio,
             ILogger<ProductService> logger
        )
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepositorio;
        }

        public async Task<PaginadoResponse<ProductDto>> BusquedaPaginado(PaginationRequest dto)
        {
            var rs = await _productRepository.BusquedaPaginado(dto);

            var rsMapp = _mapper.Map<ICollection<ProductDto>>(rs.Data);

            var response = new PaginadoResponse<ProductDto>(rsMapp, rs.Meta);

            return response;
        }

        public async Task<OperationResult<ProductDto>> CreateAsync(ProductRequestDto saveDto)
        { 
             
            Product product = _mapper.Map<Product>(saveDto);

            await _productRepository.SaveAsync(product);
             
            return new OperationResult<ProductDto>()
            {
                Success = true,
                Data = _mapper.Map<ProductDto>(product),
                Message = "Producto creado con exito"
            };
        }

        public Task<OperationResult<ProductDto>> DisabledAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<ProductDto>> EditAsync(string id, ProductRequestDto saveDto)
        {


            Product product = await _productRepository.FindByIdAsync(Convert.ToInt32(id)) ?? throw new Exception("Motorizado no encontrado para el id " + id);



            _mapper.Map(saveDto, product);

            await _productRepository.SaveAsync(product);
             
 
            return new OperationResult<ProductDto>()
            {
                Data = _mapper.Map<ProductDto>(product),
                Message = "Producto actualizado con exito",
                Success = true
            };


        }

        public Task<IReadOnlyList<ProductDto>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDto> FindByIdAsync(string id)
        { 

            var response = await _productRepository.FindByIdAsync(Convert.ToInt32(id));

            return _mapper.Map<ProductDto>(response);
        }

        public Task<IReadOnlyList<ProductDto>> GetActivo()
        {
            throw new NotImplementedException();
        }
    }
}
