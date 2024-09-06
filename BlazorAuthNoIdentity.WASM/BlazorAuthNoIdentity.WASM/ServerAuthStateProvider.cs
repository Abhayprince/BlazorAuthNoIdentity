using BlazorAuthNoIdentity.WASM.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorAuthNoIdentity.WASM;

public class ServerAuthStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistingComponentStateSubscription _subscription;
    private readonly PersistentComponentState _componentState;

    private Task<AuthenticationState> _authenticationStateTask;

    public ServerAuthStateProvider(PersistentComponentState componentState)
    {
        _componentState = componentState;

        AuthenticationStateChanged += ServerAuthStateProvider_AuthenticationStateChanged;

        _subscription = _componentState.RegisterOnPersisting(PersistComponentStateAsync, RenderMode.InteractiveWebAssembly);
    }

    private void ServerAuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task) =>
        _authenticationStateTask = task;

    private async Task PersistComponentStateAsync()
    {
        var authState = await _authenticationStateTask;

        if (authState.User.Identity?.IsAuthenticated == true)
        {
            var loggedInUser = LoggedInUserModel.FromPrincipal(authState.User);

            _componentState.PersistAsJson(nameof(LoggedInUserModel), loggedInUser);
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
        AuthenticationStateChanged -= ServerAuthStateProvider_AuthenticationStateChanged;
    }
}
