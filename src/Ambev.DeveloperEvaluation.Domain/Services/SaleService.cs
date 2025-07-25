using DeveloperStore.Sales.Domain.Entities;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation;
using static Application.DTOs.SaleRequest;

namespace Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IValidator<CreateSaleRequest> _createSaleValidator;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository saleRepository,
            IDomainEventDispatcher eventDispatcher,
            IValidator<CreateSaleRequest> createSaleValidator,
            ILogger<SaleService> logger)
        {
            _saleRepository = saleRepository;
            _eventDispatcher = eventDispatcher;
            _createSaleValidator = createSaleValidator;
            _logger = logger;
        }

        public async Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request)
        {
            // Validate request
            await _createSaleValidator.ValidateAndThrowAsync(request);

            // Check if sale number already exists
            if (await _saleRepository.ExistsBySaleNumberAsync(request.SaleNumber))
                throw new InvalidOperationException($"Sale number {request.SaleNumber} already exists");

            // Create domain entities
            var customer = MapToCustomer(request.Customer);
            var branch = MapToBranch(request.Branch);
            var sale = new Sale(request.SaleNumber, request.SaleDate, customer, branch);

            // Add items
            foreach (var itemDto in request.Items)
            {
                var product = MapToProduct(itemDto.Product);
                sale.AddItem(product, itemDto.Quantity, itemDto.UnitPrice);
            }

            // Save to repository
            await _saleRepository.AddAsync(sale);

            // Dispatch domain events
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Sale created successfully. SaleId: {SaleId}, SaleNumber: {SaleNumber}",
                sale.Id, sale.SaleNumber);

            return MapToResponse(sale);
        }

        public async Task<SaleResponse?> GetSaleByIdAsync(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            return sale != null ? MapToResponse(sale) : null;
        }

        public async Task<SaleResponse?> GetSaleBySaleNumberAsync(string saleNumber)
        {
            var sale = await _saleRepository.GetBySaleNumberAsync(saleNumber);
            return sale != null ? MapToResponse(sale) : null;
        }

        public async Task<IEnumerable<SaleResponse>> GetAllSalesAsync()
        {
            var sales = await _saleRepository.GetAllAsync();
            return sales.Select(MapToResponse);
        }

        public async Task<IEnumerable<SaleResponse>> GetSalesWithFiltersAsync(SaleFiltersDto filters)
        {
            IEnumerable<Sale> sales;

            if (filters.CustomerId.HasValue)
                sales = await _saleRepository.GetByCustomerIdAsync(filters.CustomerId.Value);
            else if (filters.BranchId.HasValue)
                sales = await _saleRepository.GetByBranchIdAsync(filters.BranchId.Value);
            else if (filters.StartDate.HasValue && filters.EndDate.HasValue)
                sales = await _saleRepository.GetByDateRangeAsync(filters.StartDate.Value, filters.EndDate.Value);
            else
                sales = await _saleRepository.GetAllAsync();

            if (filters.IsCancelled.HasValue)
                sales = sales.Where(s => s.IsCancelled == filters.IsCancelled.Value);

            return sales.Select(MapToResponse);
        }

        public async Task<SaleResponse> UpdateSaleAsync(Guid id, UpdateSaleRequest request)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new ArgumentException("Sale not found");

            if (sale.IsCancelled)
                throw new InvalidOperationException("Cannot update a cancelled sale");

            // Clear existing items and add new ones
            // Note: In a real scenario, you might want to compare and update instead of clearing
            foreach (var item in sale.Items.Where(i => !i.IsCancelled))
            {
                sale.RemoveItem(item.Id);
            }

            foreach (var itemDto in request.Items)
            {
                var product = MapToProduct(itemDto.Product);
                sale.AddItem(product, itemDto.Quantity, itemDto.UnitPrice);
            }

            await _saleRepository.UpdateAsync(sale);
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Sale updated successfully. SaleId: {SaleId}", sale.Id);

            return MapToResponse(sale);
        }

        public async Task CancelSaleAsync(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new ArgumentException("Sale not found");

            sale.Cancel();
            await _saleRepository.UpdateAsync(sale);
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Sale cancelled successfully. SaleId: {SaleId}", sale.Id);
        }

        public async Task CancelSaleItemAsync(Guid saleId, Guid itemId)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null)
                throw new ArgumentException("Sale not found");

            sale.RemoveItem(itemId);
            await _saleRepository.UpdateAsync(sale);
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Sale item cancelled successfully. SaleId: {SaleId}, ItemId: {ItemId}",
                saleId, itemId);
        }

        public async Task<SaleResponse> AddItemToSaleAsync(Guid saleId, CreateSaleItemDto itemDto)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null)
                throw new ArgumentException("Sale not found");

            var product = MapToProduct(itemDto.Product);
            sale.AddItem(product, itemDto.Quantity, itemDto.UnitPrice);

            await _saleRepository.UpdateAsync(sale);
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Item added to sale successfully. SaleId: {SaleId}", saleId);

            return MapToResponse(sale);
        }

        public async Task<SaleResponse> UpdateSaleItemAsync(Guid saleId, Guid itemId, CreateSaleItemDto itemDto)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId);
            if (sale == null)
                throw new ArgumentException("Sale not found");

            sale.UpdateItem(itemId, itemDto.Quantity, itemDto.UnitPrice);

            await _saleRepository.UpdateAsync(sale);
            await _eventDispatcher.DispatchAsync(sale.DomainEvents);
            sale.ClearDomainEvents();

            _logger.LogInformation("Sale item updated successfully. SaleId: {SaleId}, ItemId: {ItemId}",
                saleId, itemId);

            return MapToResponse(sale);
        }

        // Mapping methods
        private Customer MapToCustomer(CustomerDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Document = dto.Document
        };

        private Branch MapToBranch(BranchDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State
        };

        private Product MapToProduct(ProductDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Sku = dto.Sku
        };

        private SaleResponse MapToResponse(Sale sale) => new(
            sale.Id,
            sale.SaleNumber,
            sale.SaleDate,
            new CustomerDto(sale.Customer.Id, sale.Customer.Name, sale.Customer.Email, sale.Customer.Document),
            new BranchDto(sale.Branch.Id, sale.Branch.Name, sale.Branch.Address, sale.Branch.City, sale.Branch.State),
            sale.TotalAmount,
            sale.IsCancelled,
            sale.CreatedAt,
            sale.UpdatedAt,
            sale.Items.Select(item => new SaleItemResponse(
                item.Id,
                new ProductDto(item.Product.Id, item.Product.Name, item.Product.Description, item.Product.Category, item.Product.Sku),
                item.Quantity,
                item.UnitPrice,
                item.DiscountPercentage,
                item.DiscountAmount,
                item.SubTotal,
                item.TotalAmount,
                item.IsCancelled
            )).ToList()
        );
    }
}

