using PersonalizedInvention.Infrastructure.Data;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PersonalizedInvention.Infrastructure.Repositories
{
    // SOLID — Single Responsibility: this class only handles Product data access
    // SOLID — Open/Closed: add new query methods without changing existing ones
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.OrderBy(p => p.Name).ToListAsync();

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category) =>
            await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();

        public async Task<IEnumerable<Product>> GetInStockAsync() =>
            await _context.Products
                .Where(p => p.Stock > 0)
                .ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.FindAsync(id);

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is not null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
