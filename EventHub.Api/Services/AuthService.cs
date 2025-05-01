using EventHub.Api.Data;
using EventHub.Api.Entities;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Api.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtService _jwtService;

        public AuthService(DataContext context, IPasswordHasher<User> passwordHasher,
                           ILogger<AuthService> logger, JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _jwtService = jwtService;
        }

        public async Task<ApiResult> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                    return ApiResult.Fail("User with this email already exists");



                var user = new User
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email {Email}", registerDto.Email);
                return ApiResult.Fail("An error occurred while registering the user. Please try again.");
            }
        }




        public async Task<ApiResult<AuthDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user is null)
                {
                    return ApiResult<AuthDto>.Fail("Invalid email");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    return ApiResult<AuthDto>.Fail("Invalid password");
                }

                var token = _jwtService.GenerateJwt(user);

                return ApiResult<AuthDto>.Success(new AuthDto(user.Id, user.Name, user.Email, token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user with email {Email}", loginDto.Email);

                return ApiResult<AuthDto>.Fail("An error occurred while logging in. Please try again.");
            }
        }

    }
}
