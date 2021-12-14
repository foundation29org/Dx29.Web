"use strict";

function clickElement(element) {
    element.click();
}
function downloadFromUrl(options) {
    var _a;
    var anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = (_a = options.fileName) !== null && _a !== void 0 ? _a : '';
    anchorElement.click();
    anchorElement.remove();
}
function downloadFromByteArray(options) {
    // The byte array in .NET is encoded to base64 string when it passes to JavaScript.
    // So we can pass that base64 encoded string to the browser as a "data URL" directly.
    var url = "data:" + options.contentType + ";base64," + options.byteArray;
    downloadFromUrl({ url: url, fileName: options.fileName });
}

window.updateMessageCallerJS = (dotnetHelper) => {
    dotnetHelper.invokeMethodAsync('Dx29.Web', 'UpdateMessageCaller');
    dotnetHelper.dispose();
}

window.modalJS = {
    open: (selector, options) => {
        $(selector + ' *').prop('disabled', false);
        $(selector).modal(options);
    },
    disable: (selector) => {
        $(selector + ' *').prop('disabled', true);
    },
    close: (selector) => {
        $(selector).modal('hide');
    }
};

window.elements = {
    getAttr: (element, name) => {
        return element.getAttribute(name);
    },
    addClass: (element, name) => {
        element.classList.add(name);
    },
    removeClass: (selector, name) => {
        $(selector).removeClass(name);
    }
};
