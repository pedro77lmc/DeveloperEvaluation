namespace API.Controllers
{
    using Application.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;
    using static Application.DTOs.SaleRequest;

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISaleService saleService, ILogger<SalesController> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new sale with the specified items
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SaleResponse>> CreateSale([FromBody] CreateSaleRequest request)
        {
            try
            {
                var result = await _saleService.CreateSaleAsync(request);
                return CreatedAtAction(nameof(GetSaleById), new { id = result.Id }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets a sale by its ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleResponse>> GetSaleById(Guid id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            return sale != null ? Ok(sale) : NotFound();
        }

        /// <summary>
        /// Gets a sale by its sale number
        /// </summary>
        [HttpGet("by-number/{saleNumber}")]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleResponse>> GetSaleBySaleNumber(string saleNumber)
        {
            var sale = await _saleService.GetSaleBySaleNumberAsync(saleNumber);
            return sale != null ? Ok(sale) : NotFound();
        }

        /// <summary>
        /// Gets all sales with optional filters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SaleResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SaleResponse>>> GetSales([FromQuery] SaleFiltersDto? filters = null)
        {
            var sales = filters != null
                ? await _saleService.GetSalesWithFiltersAsync(filters)
                : await _saleService.GetAllSalesAsync();

            return Ok(sales);
        }

        /// <summary>
        /// Updates an existing sale
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleResponse>> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request)
        {
            try
            {
                var result = await _saleService.UpdateSaleAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale {SaleId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Cancels a sale
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelSale(Guid id)
        {
            try
            {
                await _saleService.CancelSaleAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sale {SaleId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Cancels a specific item from a sale
        /// </summary>
        [HttpDelete("{saleId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelSaleItem(Guid saleId, Guid itemId)
        {
            try
            {
                await _saleService.CancelSaleItemAsync(saleId, itemId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling item {ItemId} from sale {SaleId}", itemId, saleId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Adds a new item to an existing sale
        /// </summary>
        [HttpPost("{saleId:guid}/items")]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleResponse>> AddItemToSale(Guid saleId, [FromBody] CreateSaleItemDto itemDto)
        {
            try
            {
                var result = await _saleService.AddItemToSaleAsync(saleId, itemDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Updates a specific item in a sale
        /// </summary>
        [HttpPut("{saleId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SaleResponse>> UpdateSaleItem(Guid saleId, Guid itemId, [FromBody] CreateSaleItemDto itemDto)
        {
            try
            {
                var result = await _saleService.UpdateSaleItemAsync(saleId, itemId, itemDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item {ItemId} in sale {SaleId}", itemId, saleId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
