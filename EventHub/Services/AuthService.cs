
namespace EventHub.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContext _context;
        private readonly TokenService _tokenService;
        private readonly CommonService _commonService;

        public event EventHandler<LoggedInUser> UserLoggedIn;
        public event EventHandler UserLoggedOut;

        public string Token { get; private set; }
        private LoggedInUser _currentUser;
        public LoggedInUser CurrentUser 
        { 
            get => _currentUser;
            private set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    if (_currentUser != null)
                    {
                        UserLoggedIn?.Invoke(this, _currentUser);
                    }
                    else
                    {
                        UserLoggedOut?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public AuthService(HttpClient httpClient, DatabaseContext context, TokenService tokenService, CommonService commonService)
        {
            _context = context;
            _httpClient = httpClient;
            _tokenService = tokenService;
            _commonService = commonService;
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
                    return ApiResult<AuthDto>.Fail("Login failed. Please check your credentials.");

                var result = await response.Content.ReadFromJsonAsync<ApiResult<AuthDto>>();

                if (result?.IsSuccess == true)
                {
                    await StoreAuthData(result.Data);
                    return result;
                }

                return result ?? ApiResult<AuthDto>.Fail("Unknown error occurred during login.");
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
                    return ApiResult<AuthDto>.Fail("No cached credentials found. Please connect to the internet to log in.");

                if (!_tokenService.IsTokenValid(token))
                    return ApiResult<AuthDto>.Fail("Your session has expired. Please connect to the internet to log in again.");

                var userId = _tokenService.GetUserIdFromToken(token);
                var user = await _context.GetItemByIdAsync<LoggedInUser>(userId);

                if (user == null)
                    return ApiResult<AuthDto>.Fail("User data not found locally. Please connect to the internet to log in.");

                // Set the current user and token for offline use
                CurrentUser = user;
                Token = token;
                SetHttpClientToken(token);

                return ApiResult<AuthDto>.Success(new AuthDto(
                    user.Id,
                    user.Name,
                    user.Email,
                    token
                ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Offline login failed: {ex.Message}");
                return ApiResult<AuthDto>.Fail("Failed to log in offline. Please connect to the internet to log in.");
            }
        }

        private async Task StoreAuthData(AuthDto authData)
        {
            try
            {
                await SecureStorage.SetAsync("auth_token", authData.Token);
                var loggedInUser = new LoggedInUser
                {
                    Id = authData.UserId,
                    Name = authData.Name,
                    Email = authData.Email,
                    TokenExpiration = _tokenService.GetTokenExpiration(authData.Token)
                };
                await _context.SaveItemAsync(loggedInUser);
                CurrentUser = loggedInUser;
                Token = authData.Token;
                SetHttpClientToken(authData.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error storing auth data: {ex.Message}");
                throw;
            }
        }

        public async Task InitializeSessionAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    CurrentUser = null;
                    return;
                }

                if (_tokenService.IsTokenValid(token))
                {
                    SetHttpClientToken(token);
                    var userId = _tokenService.GetUserIdFromToken(token);
                    CurrentUser = await _context.GetItemByIdAsync<LoggedInUser>(userId);
                    Token = token;

                    if (_tokenService.ShouldRenewToken(token) && IsOnline())
                    {
                        await TryRefreshToken(token);
                    }
                }
                else
                {
                    await ClearAuthData();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Session init error: {ex.Message}");
                await ClearAuthData();
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

        private async Task TryRefreshToken(string oldToken)
        {
            try
            {
                if (!IsOnline()) return;

                var userId = _tokenService.GetUserIdFromToken(oldToken);
                var response = await _httpClient.PostAsync($"{AppConstants.BaseUrl}/api/auth/refresh-token", null);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<AuthDto>>();
                    if (result?.IsSuccess == true)
                    {
                        await StoreAuthData(result.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Token refresh failed: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            await ClearAuthData();
        }

        private async Task ClearAuthData()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                await _context.DeleteAllItemsAsync<LoggedInUser>();
                CurrentUser = null;
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Clear auth data error: {ex.Message}");
            }
        }
    }
}
