using EventHub.Api.Data;
using EventHub.Api.Entities;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EventHub.Api.Services
{
    public class ProfileService
    {
        private readonly DataContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;


        public ProfileService(DataContext context, JwtService jwtService,
                              IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }


        public async Task<ApiResult<string>> UpdateNameAsync(string name, int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return ApiResult<string>.Fail("User not found.");

                if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                    return ApiResult<string>.Fail("Name must be at least 3 characters long.");

                user.Name = name.Trim(); 
                await _context.SaveChangesAsync();

                var newJwt = _jwtService.GenerateJwt(user);
                return ApiResult<string>.Success(newJwt);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail($"An error occurred: {ex.Message}");
            }
        }


        public async Task<ApiResult> ChangePasswordAsync(string newPassword, int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return ApiResult.Fail("User not found.");

                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                    return ApiResult.Fail("Password must be at least 8 characters long.");

               

                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
                await _context.SaveChangesAsync();

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}
