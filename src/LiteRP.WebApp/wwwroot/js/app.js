window.LiteRP = {
    openFileDialog: function (inputId) {
        document.getElementById(inputId).click();
    },
    scrollToBottom: function scrollToBottom() {
        window.scrollTo(0, document.body.scrollHeight);
    }
};