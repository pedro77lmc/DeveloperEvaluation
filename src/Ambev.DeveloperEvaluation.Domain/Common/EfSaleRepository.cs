using DeveloperStore.Sales.Domain.Entities;
using Domain.Repositories;
using System.Data.Entity;

namespace Infra.Repositories
{
    public class EfSaleRepository : ISaleRepository
    {
        private readonly SalesDbContext _context;

        public EfSaleRepository(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<Sale?> GetByIdAsync(Guid id) => await _context.GetSales()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<Sale?> GetBySaleNumberAsync(string saleNumber)
        {
            return await _context.GetSales()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.GetSales()
                .Include(s => s.Items)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.GetSales()
                .Include(s => s.Items)
                .Where(s => s.Customer.Id == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId)
        {
            return await _context.GetSales()
                .Include(s => s.Items)
                .Where(s => s.Branch.Id == branchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.GetSales()
                .Include(s => s.Items)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();
        }

        public async Task AddAsync(Sale sale)
        {
            _context.GetSales().Add(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sale sale)
        {
            _context.GetSales().Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsBySaleNumberAsync(string saleNumber)
        {
            return await _context.GetSales().AnyAsync(s => s.SaleNumber == saleNumber);
        }
    }

}
