using System.Security.Claims;

namespace BlazorAuthNoIdentity.WASM.Client;

public record LoggedInUserModel(int Id, string Name, string Email)
{
    public Claim[] ToClaims() =>
        [
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.Email, Email),
        ];

    public static LoggedInUserModel FromPrincipal(ClaimsPrincipal principal)
    {
        var userId = Convert.ToInt32(principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var name = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        return new LoggedInUserModel(userId, name, email);
    }
}
