using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHub.Api.Controllers
{
    [Authorize]
    public class AppBaseController : ControllerBase
    {
        private int? _userId;

        public int UserId => _userId ??=
            Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}
