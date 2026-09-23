using PersonalizedInvention.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Interfaces
{
    public interface IAdminService
    {
        // Products
        Task<IEnumerable<AdminProductDto>> GetAllProductsAsync();
        Task<AdminProductDto> CreateProductAsync(CreateProductDto dto);
        Task<AdminProductDto?> UpdateProductAsync(int id, CreateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int id, int stock);

        // Orders
        Task<IEnumerable<AdminOrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}
