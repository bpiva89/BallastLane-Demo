using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Interfaces;
using BallastLane.ProductManagement.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ProductManagement.API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _service;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public ProductsController(ILogger<ProductsController> logger, IProductService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Get a paged list of products.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetPaged(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("ProductsController.GetPaged Started. PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

            var result = await _service.GetPagedAsync(pageNumber, pageSize);

            if (result.IsFailure)
            {
                _logger.LogWarning("ProductsController.GetPaged Failed: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("ProductsController.GetPaged Finished. Count={Count}", result.Value.Items.Count);

            return Ok(result.Value);
        }

        /// <summary>
        /// Get all products (un-paged, for specific clients/reports).
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IList<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<ProductDto>>> GetAll()
        {
            _logger.LogInformation("ProductsController.GetAll Started.");

            var result = await _service.GetAllAsync();

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            _logger.LogInformation("ProductsController.GetAll Finished count={Count}.", result.Value.Count);

            return Ok(result.Value);
        }

        /// <summary>
        /// Get a product by Id.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            _logger.LogInformation("ProductsController.GetById Started id={Id}.", id);

            var result = await _service.GetByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogWarning("ProductsController.GetById NotFound id={Id}.", id);
                return NotFound(result.Error);
            }

            _logger.LogInformation("ProductsController.GetById Finished id={Id}.", id);

            return Ok(result.Value);
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("ProductsController.Create Started name={Name}.", dto.Name);

            var result = await _service.CreateAsync(dto);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            _logger.LogInformation("ProductsController.Create Finished id={Id}.", result.Value.Id);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("ProductsController.Update Started id={Id}.", id);

            var result = await _service.UpdateAsync(id, dto);

            if (result.IsFailure)
            {
                if (result.Error.Contains("was not found"))
                {
                    return NotFound(result.Error);
                }
                return BadRequest(result.Error);
            }

            _logger.LogInformation("ProductsController.Update Finished id={Id}.", id);

            return Ok(result.Value);
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("ProductsController.Delete Started id={Id}.", id);

            var result = await _service.DeleteAsync(id);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            _logger.LogInformation("ProductsController.Delete Finished id={Id}.", id);

            return NoContent();
        }
    }
}
