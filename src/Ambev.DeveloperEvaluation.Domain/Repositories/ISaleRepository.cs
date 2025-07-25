using DeveloperStore.Sales.Domain.Entities;

namespace Domain.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale?> GetByIdAsync(Guid id);
        Task<Sale?> GetBySaleNumberAsync(string saleNumber);
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId);
        Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddAsync(Sale sale);
        Task UpdateAsync(Sale sale);
        Task<bool> ExistsBySaleNumberAsync(string saleNumber);
    }
}
