using EventHub.Api.Services;
using EventHub.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Api.Controllers
{
    [Route("api/me")]
    [ApiController]
    public class ProfileController : AppBaseController
    {
        private readonly ProfileService _profileService;
        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPut("update-name")] // PUT api/me/update-name
        public async Task<ApiResult<string>> UpdateName([FromBody] UpdateNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResult<string>.Fail("Name cannot be empty.");

            return await _profileService.UpdateNameAsync(dto.Name, UserId);
        }


        [HttpPut("change-password")] // PUT api/me/change-password
        public async Task<ApiResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return ApiResult.Fail("Password cannot be empty.");

            return await _profileService.ChangePasswordAsync(dto.NewPassword, UserId);
        }
    }

}
