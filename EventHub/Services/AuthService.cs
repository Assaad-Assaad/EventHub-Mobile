
using EventHub.Data;
using EventHub.Models;
using EventHub.Shared.Dtos;
using EventHub.Utils;
using EventHub.Views;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EventHub.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        private readonly TokenService _tokenService;

        
        public string Token { get; private set; }
        public LoggedInUser CurrentUser { get; private set; }  = new();

        public AuthService(HttpClient httpClient, DatabaseContext context, TokenService tokenService)
        {
            _context = context;
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        private bool IsOnline() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;



        public async Task<ApiResult> RegisterAsync(RegisterDto dto)
        {
            if (!IsOnline())
            {
                return ApiResult.Fail("No Internet Connection");
            }

            var response = await _httpClient.PostAsJsonAsync($"{AppConstants.BaseUrl}/api/auth/register", dto);
            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Success();
            }
            return ApiResult.Fail("Registration failed");
        }


        public async Task<ApiResult<AuthDto>> LoginAsync(LoginDto dto)
        {
            if (IsOnline())
            {
                var response = await _httpClient.PostAsJsonAsync($"{AppConstants.BaseUrl}/api/auth/login", dto);

                if (!response.IsSuccessStatusCode)
                    return ApiResult<AuthDto>.Fail("Login failed");

                
                var result = await response.Content.ReadFromJsonAsync<ApiResult<AuthDto>>();

                if (result?.IsSuccess == true)
                {
                    await StoreAuthData(result.Data);
                    return result;
                }

                return result ?? ApiResult<AuthDto>.Fail("Unknown error");
            }
            else
            {
                return await TryOfflineLogin();
            }
        }


        public async Task<ApiResult<AuthDto>> TryOfflineLogin()
        {
            try
            {
                
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                    return ApiResult<AuthDto>.Fail("No cached credentials");

                
                var tokenService = new TokenService();
                if (!tokenService.IsTokenValid(token))
                    return ApiResult<AuthDto>.Fail("Token expired or invalid");

                
                var userId = tokenService.GetUserIdFromToken(token);
                var user = await _context.GetItemByIdAsync<LoggedInUser>(userId);

                if (user == null)
                    return ApiResult<AuthDto>.Fail("User not found locally");

                
                return ApiResult<AuthDto>.Success(new AuthDto(
                    user.Id,
                    user.Name,
                    user.Email,
                    token
                ));
            }
            catch (Exception ex)
            {

                return ApiResult<AuthDto>.Fail($"Offline login failed: {ex.Message}");
            }
        }


        private async Task StoreAuthData(AuthDto authData)
        {
            
            await SecureStorage.SetAsync("auth_token", authData.Token);
            await _context.SaveItemAsync(new LoggedInUser
            {
                Id = authData.UserId,
                Name = authData.Name,
                Email = authData.Email,
                TokenExpiration = _tokenService.GetTokenExpiration(authData.Token)
            });
        }

        public async Task InitializeSessionAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                if (_tokenService.IsTokenValid(token))
                {
                    SetHttpClientToken(token); // Set token for API calls
                    var userId = _tokenService.GetUserIdFromToken(token);
                    CurrentUser = await _context.GetItemByIdAsync<LoggedInUser>(userId);
                }
                else
                {
                    await ClearAuthData();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Session init error: {ex.Message}");
            }
        }

        private void SetHttpClientToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return;

                
                _httpClient.DefaultRequestHeaders.Remove("Authorization");

                
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error setting token: {ex.Message}");
            }
        }


        public async Task ClearAuthData()
        {
            SecureStorage.Remove("auth_token");
            await _context.DeleteAllItemsAsync<LoggedInUser>();
            CurrentUser = null;
        }

        public async Task LogoutAsync()
        {
            await ClearAuthData();


            await Shell.Current.GoToAsync($"//{nameof(AuthPage)}");
        }

    }
}
