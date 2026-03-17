using Application.Dtos.Product;
using Application.Mappers;
using Application.Services;
using AutoMapper;
using Domain;
using Domain.Entities;
using Infraestructure.Persistences.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Application.Emails.Services.Interfaces;

namespace Application.Tests;

public class ProductServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ProductMappingsProfile());
        });

        return config.CreateMapper();
    }

    [Fact]
    public async Task CreateAsync_CreaProducto_Y_RetornaOperacionExitosa()
    {
        var mapper = CreateMapper();

        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        var movRepo = new Mock<IMovimientoRepository>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);

        productRepo
            .Setup(r => r.SaveAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.Id = 1;
                return p;
            });

        var service = new ProductService(mapper, productRepo.Object, movRepo.Object, emailService.Object, NullLogger<ProductService>.Instance);

        var request = new ProductRequestDto
        {
            Name = "Producto 1",
            Description = "Desc",
            Price = 10,
            Stock = 2,
            Categorie = "Cat"
        };

        var result = await service.CreateAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Producto 1", result.Data!.Name);
        Assert.Equal(1, result.Data!.Id);

        productRepo.Verify(r => r.SaveAsync(It.IsAny<Product>()), Times.Once);
        movRepo.VerifyNoOtherCalls();
        emailService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task EditAsync_ActualizaProducto_Y_RetornaOperacionExitosa()
    {
        var mapper = CreateMapper();

        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        var movRepo = new Mock<IMovimientoRepository>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);

        var existing = new Product
        {
            Id = 10,
            Name = "Old",
            Description = "Old",
            Price = 1,
            Stock = 1,
            Categorie = "Old"
        };

        productRepo
            .Setup(r => r.FindByIdAsync(10))
            .ReturnsAsync(existing);

        productRepo
            .Setup(r => r.SaveAsync(existing))
            .ReturnsAsync(existing);

        var service = new ProductService(mapper, productRepo.Object, movRepo.Object, emailService.Object, NullLogger<ProductService>.Instance);

        var request = new ProductRequestDto
        {
            Name = "New",
            Description = "New Desc",
            Price = 99,
            Stock = 5,
            Categorie = "NewCat"
        };

        var result = await service.EditAsync("10", request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(10, result.Data!.Id);
        Assert.Equal("New", result.Data!.Name);
        Assert.Equal(5, result.Data!.Stock);

        productRepo.Verify(r => r.FindByIdAsync(10), Times.Once);
        productRepo.Verify(r => r.SaveAsync(existing), Times.Once);
        movRepo.VerifyNoOtherCalls();
        emailService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FindByIdAsync_RetornaProductoMapeado()
    {
        var mapper = CreateMapper();

        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        var movRepo = new Mock<IMovimientoRepository>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);

        var existing = new Product
        {
            Id = 7,
            Name = "P7",
            Description = "D7",
            Price = 12,
            Stock = 3,
            Categorie = "C7"
        };

        productRepo
            .Setup(r => r.FindByIdAsync(7))
            .ReturnsAsync(existing);

        var service = new ProductService(mapper, productRepo.Object, movRepo.Object, emailService.Object, NullLogger<ProductService>.Instance);

        var dto = await service.FindByIdAsync("7");

        Assert.NotNull(dto);
        Assert.Equal(7, dto.Id);
        Assert.Equal("P7", dto.Name);

        productRepo.Verify(r => r.FindByIdAsync(7), Times.Once);
        movRepo.VerifyNoOtherCalls();
        emailService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BusquedaPaginado_RetornaRespuestaPaginadaMapeada()
    {
        var mapper = CreateMapper();

        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        var movRepo = new Mock<IMovimientoRepository>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);

        var req = new PaginationRequest
        {
            Page = 1,
            Take = 2,
            Sort = "",
            Filters = Array.Empty<string>()
        };

        var data = new List<Product>
        {
            new() { Id = 1, Name = "A", Description = "A", Price = 1, Stock = 1, Categorie = "C" },
            new() { Id = 2, Name = "B", Description = "B", Price = 2, Stock = 2, Categorie = "C" }
        };

        var paged = new PaginadoResponse<Product>(data, new Meta { Page = 1, TotalCount = 2, TotalPages = 1 });

        productRepo
            .Setup(r => r.BusquedaPaginado(It.IsAny<PaginationRequest>()))
            .ReturnsAsync(paged);

        var service = new ProductService(mapper, productRepo.Object, movRepo.Object, emailService.Object, NullLogger<ProductService>.Instance);

        var result = await service.BusquedaPaginado(req);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Meta.TotalCount);

        productRepo.Verify(r => r.BusquedaPaginado(It.IsAny<PaginationRequest>()), Times.Once);
        movRepo.VerifyNoOtherCalls();
        emailService.VerifyNoOtherCalls();
    }
}
