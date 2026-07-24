using System.Security.Claims;

namespace TaskManagement.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetRequiredUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Token geçerli bir kullanıcı kimliği içermiyor.");

            return userId;
        }
    }
}
