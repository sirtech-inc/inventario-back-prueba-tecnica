using Application.Dtos.Product;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<Results<BadRequest, Ok<PaginadoResponse<ProductDto>>>> BusquedaPaginado([FromQuery] PaginationRequest dto)
        {
            var response = await _productService.BusquedaPaginado(dto);

            if (response != null)
            {
                return TypedResults.Ok(response);
            }

            return TypedResults.BadRequest();
        }

        [HttpGet("low-stock")]
        public async Task<Results<BadRequest, Ok<PaginadoResponse<ProductDto>>>> BusquedaPaginadoStockBajo([FromQuery] PaginationRequest dto)
        {
            var response = await _productService.BusquedaPaginadoStockBajo(dto);

            if (response != null)
            {
                return TypedResults.Ok(response);
            }

            return TypedResults.BadRequest();
        }

        [HttpGet("low-stock/pdf")]
        public async Task<IActionResult> LowStockPdf()
        {
            var items = await _productService.GetStockBajoAsync(5);

            using var document = new PdfDocument();
            document.Info.Title = "Productos con stock bajo";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
            var headerFont = new XFont("Arial", 10, XFontStyle.Bold);
            var rowFont = new XFont("Arial", 10, XFontStyle.Regular);

            const double margin = 40;
            double y = margin;

            gfx.DrawString("Productos con stock <= 5", titleFont, XBrushes.Black,
                new XRect(margin, y, page.Width - margin * 2, 20), XStringFormats.TopLeft);
            y += 28;

            gfx.DrawString($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}", rowFont, XBrushes.Black,
                new XRect(margin, y, page.Width - margin * 2, 15), XStringFormats.TopLeft);
            y += 22;

            void DrawHeader()
            {
                gfx.DrawLine(XPens.Black, margin, y, page.Width - margin, y);
                y += 6;

                gfx.DrawString("Id", headerFont, XBrushes.Black, new XRect(margin, y, 40, 15), XStringFormats.TopLeft);
                gfx.DrawString("Nombre", headerFont, XBrushes.Black, new XRect(margin + 45, y, 230, 15), XStringFormats.TopLeft);
                gfx.DrawString("Stock", headerFont, XBrushes.Black, new XRect(margin + 280, y, 60, 15), XStringFormats.TopLeft);
                gfx.DrawString("Precio", headerFont, XBrushes.Black, new XRect(margin + 345, y, 70, 15), XStringFormats.TopLeft);
                gfx.DrawString("Categoría", headerFont, XBrushes.Black, new XRect(margin + 420, y, page.Width - margin - (margin + 420), 15), XStringFormats.TopLeft);

                y += 18;
                gfx.DrawLine(XPens.Black, margin, y, page.Width - margin, y);
                y += 8;
            }

            DrawHeader();

            foreach (var p in items)
            {
                if (y > page.Height - margin - 20)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;
                    DrawHeader();
                }

                gfx.DrawString(p.Id.ToString(), rowFont, XBrushes.Black, new XRect(margin, y, 40, 15), XStringFormats.TopLeft);
                var safeName = p.Name ?? string.Empty;
                gfx.DrawString(safeName.Length > 35 ? safeName.Substring(0, 35) + "..." : safeName,
                    rowFont, XBrushes.Black, new XRect(margin + 45, y, 230, 15), XStringFormats.TopLeft);
                gfx.DrawString(p.Stock.ToString(), rowFont, XBrushes.Black, new XRect(margin + 280, y, 60, 15), XStringFormats.TopLeft);
                gfx.DrawString(p.Price.ToString("0.00"), rowFont, XBrushes.Black, new XRect(margin + 345, y, 70, 15), XStringFormats.TopLeft);
                gfx.DrawString(p.Categorie ?? string.Empty, rowFont, XBrushes.Black,
                    new XRect(margin + 420, y, page.Width - margin - (margin + 420), 15), XStringFormats.TopLeft);

                y += 16;
            }

            using var stream = new MemoryStream();
            document.Save(stream, false);

            return File(stream.ToArray(), "application/pdf", "low-stock-products.pdf");
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
        public async Task<Results<BadRequest, Ok<OperationResult<ProductDto>>>> Post([FromBody] ProductRequestDto request)
        {
            var response = await _productService.CreateAsync(request);

            if (response != null) return TypedResults.Ok(response);

            return TypedResults.BadRequest();
        }

        [HttpPut("{id}")]
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

        [HttpPost("{id}/stock")]
        public async Task<Results<BadRequest<OperationResult<ProductDto>>, Ok<OperationResult<ProductDto>>>> AjustarStock(
            string id,
            [FromBody] StockMoveRequestDto request)
        {
            var response = await _productService.AjustarStockAsync(id, request);

            if (response.Success) return TypedResults.Ok(response);

            return TypedResults.BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<Results<BadRequest<OperationResult<bool>>, Ok<OperationResult<bool>>>> Delete(string id)
        {
            var response = await _productService.DeleteAsync(id);

            if (response.Success) return TypedResults.Ok(response);

            return TypedResults.BadRequest(response);
        }
    }
}
