


namespace EventHub.Services
{
    public class TokenService
    {
        public bool IsTokenValid(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return int.Parse(jwt.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public DateTime GetTokenExpiration(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo;
        }


        public bool ShouldRenewToken(string token)
        {
            var expiration = GetTokenExpiration(token);
            return expiration < DateTime.UtcNow.AddMinutes(30);
        }
    }
}
