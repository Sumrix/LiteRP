window.LiteRP = window.LiteRP || {};

window.LiteRP.autoScroll = (function () {
    let observer = null;
    let element = null;
    let isScrolledToBottom = true;

    function handleScroll() {
        if (!element) return;
        const threshold = 50;
        isScrolledToBottom = element.scrollTop + element.clientHeight >= element.scrollHeight - threshold;
    }

    // Возвращаем публичные методы
    return {
        start: function (elementId) {
            element = document.getElementById(elementId);
            if (!element) {
                console.error(`AutoScroll: Element with id '${elementId}' not found.`);
                return;
            }

            element.scrollTop = element.scrollHeight;
            isScrolledToBottom = true;
            element.addEventListener('scroll', handleScroll);

            observer = new MutationObserver(() => {
                if (isScrolledToBottom) {
                    element.scrollTop = element.scrollHeight;
                }
            });

            observer.observe(element, { childList: true, subtree: true });
            console.log(`AutoScroll observer started for #${elementId}.`);
        },
        stop: function () {
            if (observer) {
                observer.disconnect();
                observer = null;
            }
            if (element) {
                element.removeEventListener('scroll', handleScroll);
                element = null;
            }
            console.log("AutoScroll observer stopped.");
        }
    };
})();