using Microsoft.AspNetCore.Components;

namespace LiteRP.WebApp.Services;

public interface IElementClickObserverService
{
    /// <summary>
    /// Subscribes an element to receive notifications for clicks outside of it.
    /// </summary>
    /// <param name="element">The element reference to watch.</param>
    /// <param name="onClickOutside">The action to execute when a click outside occurs.</param>
    /// <returns>A unique subscription ID to be used for unsubscribing.</returns>
    Task<Guid> Subscribe(ElementReference element, Action onClickOutside);

    /// <summary>
    /// Unsubscribes an element from receiving notifications.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID returned by Subscribe.</param>
    Task Unsubscribe(Guid subscriptionId);
}