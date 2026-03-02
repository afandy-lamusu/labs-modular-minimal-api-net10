using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Api.Modules.Products.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) 
        => await _context.Products.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct) 
        => await _context.Products.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct) 
        => await _context.Products.AddAsync(product, ct);

    public async Task SaveChangesAsync(CancellationToken ct) 
        => await _context.SaveChangesAsync(ct);
}
