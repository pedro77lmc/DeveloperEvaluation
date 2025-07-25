using static Application.DTOs.SaleRequest;

namespace Application.Services
{
    public interface ISaleService
    {
        Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request);
        Task<SaleResponse?> GetSaleByIdAsync(Guid id);
        Task<SaleResponse?> GetSaleBySaleNumberAsync(string saleNumber);
        Task<IEnumerable<SaleResponse>> GetAllSalesAsync();
        Task<IEnumerable<SaleResponse>> GetSalesWithFiltersAsync(SaleFiltersDto filters);
        Task<SaleResponse> UpdateSaleAsync(Guid id, UpdateSaleRequest request);
        Task CancelSaleAsync(Guid id);
        Task CancelSaleItemAsync(Guid saleId, Guid itemId);
        Task<SaleResponse> AddItemToSaleAsync(Guid saleId, CreateSaleItemDto itemDto);
        Task<SaleResponse> UpdateSaleItemAsync(Guid saleId, Guid itemId, CreateSaleItemDto itemDto);

    }
}
