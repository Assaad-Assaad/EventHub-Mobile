using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public class UpdateNameDto
    {
        public string Name { get; set; }
    }

    public class ChangePasswordDto
    {
        public string NewPassword { get; set; }
    }
}
