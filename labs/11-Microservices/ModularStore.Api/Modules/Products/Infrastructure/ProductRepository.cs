using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Api.Modules.Products.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) 
        => await _context.Products.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct) 
        => await _context.Products
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct) 
        => await _context.Products.AddAsync(product, ct);

    public async Task SaveChangesAsync(CancellationToken ct) 
        => await _context.SaveChangesAsync(ct);
}
