using Core.DTOs;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;



namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;


    public UserRepository(AppDbContext context, IMapper mapper )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<User?> GetByIdAsync(string id){
        var userFound = await _context.Users
            .FirstOrDefaultAsync(r => r.Id == id);

        if (userFound != null)
        {
            throw new ArgumentException("User cannot found.");
        }
        return userFound;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(){
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);  
    }

    public async Task CreateAsync(UserCreateDto user){
        var newUser = new User {
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Role
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto> UpdateAsync(string userId, UserCreateDto user)
    {
        var existingUser = await _context.Users.FindAsync(userId);
        if (existingUser == null)
        {
            throw new ArgumentException($"No user found with ID '{userId}'.");
        }
        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.Role = user.Role;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(existingUser);    
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(s => s.Id == id );
            
        if (await _context.FaultReports.AnyAsync(r => r.UserId == id))
        {
            throw new ArgumentException("User cannot delete because this user has reports.");
        }
        if (user == null)
        {
            throw new ArgumentException($"No user found with ID '{id}'.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    
}