using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LiteRP.WebApp.Services;

public class ElementClickObserverService : IElementClickObserverService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ConcurrentDictionary<Guid, Action> _subscriptions = new();
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<ElementClickObserverService>? _dotNetObjectRef;

    public ElementClickObserverService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task InitializeAsync()
    {
        if (_jsModule is null)
        {
            _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/click-observer.js");
            _dotNetObjectRef = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", _dotNetObjectRef);
        }
    }

    public async Task<Guid> Subscribe(ElementReference element, Action onClickOutside)
    {
        // Ensure the JS module is initialized before subscribing
        // This is a simplified lazy initialization
        await InitializeAsync();
        
        var subscriptionId = Guid.NewGuid();
        _subscriptions[subscriptionId] = onClickOutside;
        await _jsModule!.InvokeVoidAsync("register", element, subscriptionId);
        return subscriptionId;
    }

    public async Task Unsubscribe(Guid subscriptionId)
    {
        if (_subscriptions.TryRemove(subscriptionId, out _))
        {
            if (_jsModule is not null)
            {
                await _jsModule.InvokeVoidAsync("unregister", subscriptionId);
            }
        }
    }

    [JSInvokable]
    public void OnDocumentClick(string subscriptionId)
    {
        if (Guid.TryParse(subscriptionId, out var id) && _subscriptions.TryGetValue(id, out var action))
        {
            action.Invoke();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            try
            {
                // Unregistering all is not strictly necessary as the JS context will be destroyed,
                // but it's good practice if the service's lifetime was shorter.
                await _jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException) { /* Ignore */ }
        }
        _dotNetObjectRef?.Dispose();
    }
}