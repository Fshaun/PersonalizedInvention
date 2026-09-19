using PersonalizedInvention.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateAsync(User user);
    }
}
