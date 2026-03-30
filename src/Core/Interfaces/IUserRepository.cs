using Core.Entities;
using Core.DTOs;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task CreateAsync(UserCreateDto user);
    Task<UserDto> UpdateAsync(string userId, UserCreateDto user);
    Task DeleteAsync(string id);
}