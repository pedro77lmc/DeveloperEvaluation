using DeveloperStore.Sales.Domain.Entities;
using Domain.Repositories;

namespace Infra.Repositories
{
    public class InMemorySaleRepository : ISaleRepository
    {
        private readonly List<Sale> _sales = new();
        private readonly object _lock = new();

        public Task<Sale?> GetByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var sale = _sales.FirstOrDefault(s => s.Id == id);
                return Task.FromResult(sale);
            }
        }

        public Task<Sale?> GetBySaleNumberAsync(string saleNumber)
        {
            lock (_lock)
            {
                var sale = _sales.FirstOrDefault(s => s.SaleNumber == saleNumber);
                return Task.FromResult(sale);
            }
        }

        public Task<IEnumerable<Sale>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_sales.AsEnumerable());
            }
        }

        public Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
        {
            lock (_lock)
            {
                var sales = _sales.Where(s => s.Customer.Id == customerId);
                return Task.FromResult(sales);
            }
        }

        public Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId)
        {
            lock (_lock)
            {
                var sales = _sales.Where(s => s.Branch.Id == branchId);
                return Task.FromResult(sales);
            }
        }

        public Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            lock (_lock)
            {
                var sales = _sales.Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);
                return Task.FromResult(sales);
            }
        }

        public Task AddAsync(Sale sale)
        {
            lock (_lock)
            {
                _sales.Add(sale);
                return Task.CompletedTask;
            }
        }

        public Task UpdateAsync(Sale sale)
        {
            lock (_lock)
            {
                var existingSale = _sales.FirstOrDefault(s => s.Id == sale.Id);
                if (existingSale != null)
                {
                    var index = _sales.IndexOf(existingSale);
                    _sales[index] = sale;
                }
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsBySaleNumberAsync(string saleNumber)
        {
            lock (_lock)
            {
                var exists = _sales.Any(s => s.SaleNumber == saleNumber);
                return Task.FromResult(exists);
            }
        }
    }

}
