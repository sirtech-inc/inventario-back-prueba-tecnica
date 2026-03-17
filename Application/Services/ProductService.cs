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
        private readonly IMovimientoRepository _movimientoRepository;

        public ProductService(
            IMapper mapper,
            IProductRepository productRepositorio,
            IMovimientoRepository movimientoRepository,
             ILogger<ProductService> logger
        )
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepositorio;
            _movimientoRepository = movimientoRepository;
        }

        public async Task<PaginadoResponse<ProductDto>> BusquedaPaginado(PaginationRequest dto)
        {
            var rs = await _productRepository.BusquedaPaginado(dto);

            var rsMapp = _mapper.Map<ICollection<ProductDto>>(rs.Data);

            var response = new PaginadoResponse<ProductDto>(rsMapp, rs.Meta);

            return response;
        }

        public async Task<PaginadoResponse<ProductDto>> BusquedaPaginadoStockBajo(PaginationRequest dto)
        {
            var rs = await _productRepository.BusquedaPaginadoStockBajo(dto, 5);

            var rsMapp = _mapper.Map<ICollection<ProductDto>>(rs.Data);

            var response = new PaginadoResponse<ProductDto>(rsMapp, rs.Meta);

            return response;
        }

        public async Task<IReadOnlyList<ProductDto>> GetStockBajoAsync(int maxStock = 5)
        {
            var rs = await _productRepository.GetStockBajoAsync(maxStock);
            return _mapper.Map<IReadOnlyList<ProductDto>>(rs);
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

        public async Task<OperationResult<ProductDto>> AjustarStockAsync(string id, StockMoveRequestDto request)
        {
            if (request == null)
                return OperationResult<ProductDto>.Fail("Request inválido.");

            if (request.Quantity <= 0)
                return OperationResult<ProductDto>.Fail("La cantidad debe ser mayor a 0.");

            var type = (request.TypeMove ?? string.Empty).Trim().ToLowerInvariant();
            if (type != "aumentar" && type != "disminuir")
                return OperationResult<ProductDto>.Fail("TypeMove inválido. Use: aumentar | disminuir");

            await using var tx = await _productRepository.BeginTransactionAsync();
            try
            {
                var productId = Convert.ToInt32(id);
                var product = await _productRepository.FindByIdAsync(productId);

                if (product == null)
                    return OperationResult<ProductDto>.Fail("Producto no encontrado.");

                if (type == "disminuir" && product.Stock < request.Quantity)
                    return OperationResult<ProductDto>.Fail("Stock insuficiente para disminuir.");

                product.Stock = type == "aumentar"
                    ? product.Stock + request.Quantity
                    : product.Stock - request.Quantity;

                await _productRepository.SaveAsync(product);

                var movimiento = new Movimiento
                {
                    ProductId = product.Id,
                    Quantity = request.Quantity,
                    TypeMove = type
                };

                await _movimientoRepository.AddAsync(movimiento);
                await _movimientoRepository.SaveChangesAsync();

                await tx.CommitAsync();

                return OperationResult<ProductDto>.Ok("Stock actualizado con éxito", _mapper.Map<ProductDto>(product));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error ajustando stock para producto {ProductId}", id);
                return OperationResult<ProductDto>.Fail("Ocurrió un error al actualizar el stock.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(string id)
        {
            await using var tx = await _productRepository.BeginTransactionAsync();
            try
            {
                var productId = Convert.ToInt32(id);
                var product = await _productRepository.FindByIdAsync(productId);

                if (product == null)
                    return OperationResult<bool>.Fail("Producto no encontrado.");

                var movimientos = await _movimientoRepository.FindAllByPropertyAsync("ProductId", productId);

                if (movimientos != null && movimientos.Count > 0)
                {
                    await _movimientoRepository.DeleteRangeAsync(movimientos);
                }

                await _productRepository.DeleteAsync(product);

                await tx.CommitAsync();

                return OperationResult<bool>.Ok("Producto eliminado con éxito", true);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error eliminando producto {ProductId}", id);
                return OperationResult<bool>.Fail("Ocurrió un error al eliminar el producto.");
            }
        }
    }
}
