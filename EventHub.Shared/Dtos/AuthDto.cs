using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public record AuthDto(int UserId, string Name, string Email, string Token);
}
