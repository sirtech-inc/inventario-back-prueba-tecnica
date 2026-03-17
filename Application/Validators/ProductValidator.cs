using Application.Dtos.Product;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class ProductValidator : AbstractValidator<ProductRequestDto> 
    {
        public ProductValidator()
        {
            RuleFor(x  => x.Name).NotNull().WithMessage("El campo Nombre es requerido")
                .NotEmpty().WithMessage("El campo Nombre es requerido");
            RuleFor(x => x.Price).NotNull().WithMessage("El campo Precio es requerido")
            .NotEmpty().WithMessage("El campo Precio es requerido");
        }
    }
}
