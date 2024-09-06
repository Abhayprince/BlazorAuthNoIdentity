using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuthNoIdentity.WASM.Client;

public class ClientAuthStateProvider : AuthenticationStateProvider
{
    private readonly Task<AuthenticationState> _authenticationStateTask = 
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public ClientAuthStateProvider(PersistentComponentState componentState)
    {
        ClaimsPrincipal principal;
        if (componentState.TryTakeFromJson<LoggedInUserModel>(nameof(LoggedInUserModel), out var loggedInUser) && loggedInUser is not null && loggedInUser.Id > 0)
        {
            var claims = loggedInUser.ToClaims();
            var identity = new ClaimsIdentity(claims, Constants.AuthScheme);
            principal = new ClaimsPrincipal(identity);
        }
        else
        {
            var identity = new ClaimsIdentity();
            principal = new ClaimsPrincipal(identity);            
        }

        var authState = new AuthenticationState(principal);
        _authenticationStateTask = Task.FromResult(authState);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        _authenticationStateTask;
}
