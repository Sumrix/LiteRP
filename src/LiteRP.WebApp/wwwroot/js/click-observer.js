let dotNetHelper;
const registeredElements = new Map();

// Initializes the module with a .NET helper and sets up the single global listener
export function initialize(helper) {
    dotNetHelper = helper;
    document.addEventListener('click', (event) => {
        // When a click occurs, check all registered elements
        for (const [element, subscriptionId] of registeredElements.entries()) {
            // If the click happened outside the element, notify .NET
            if (!element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('OnDocumentClick', subscriptionId.toString());
            }
        }
    }, true); // Use capture phase to handle all clicks reliably
}

// Adds an element to the watch list
export function register(element, subscriptionId) {
    if (element) {
        registeredElements.set(element, subscriptionId);
    }
}

// Removes an element from the watch list
export function unregister(subscriptionId) {
    for (const [element, id] of registeredElements.entries()) {
        if (id === subscriptionId) {
            registeredElements.delete(element);
            break;
        }
    }
}