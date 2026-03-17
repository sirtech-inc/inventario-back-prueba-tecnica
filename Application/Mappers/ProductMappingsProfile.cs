using Application.Dtos.Product;
using AutoMapper;
using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class ProductMappingsProfile : Profile
    {
        public ProductMappingsProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<PaginadoResponse<Product>, PaginadoResponse<ProductDto>>().ReverseMap();
            CreateMap<ProductRequestDto, Product>();

        }
    }
}
