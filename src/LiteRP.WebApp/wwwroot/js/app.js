window.LiteRP = {
    openFileDialog: function (inputId) {
        document.getElementById(inputId).click();
    },
    scrollToBottom: function () {
        window.scrollTo(0, document.body.scrollHeight);
    },
    submitOnEnter: (id, dotNetHelper) => {
        const elem = document.getElementById(id);
        elem._lrpKeydownListener = (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('OnSubmitFromJs');
            }
        };

        elem.addEventListener('keydown', elem._lrpKeydownListener);
    }
};