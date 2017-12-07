'use strict';

var keepAliveHook = function () {
    var _this = this,
        _arguments = arguments;

    var library = {};
    var w = window;
    w._keepAlive = false;

    w._oldOpen = XMLHttpRequest.prototype.open;
    var onStateChange = function onStateChange(event) {
        if (event.currentTarget.readyState === 4) {
            library._onHttpMonitor(event.currentTarget.responseURL, event.currentTarget.status);
        }
    };
    XMLHttpRequest.prototype.open = function () {
        // when an XHR object is opened, add a listener for its readystatechange events
        _this.addEventListener('readystatechange', onStateChange);
        // run the real `open`
        w._oldOpen.apply(_this, _arguments);
    };
    if (!window.fetch.polyfill) {
        w._oldFetch = w.fetch;

        window.fetch = function (input, init) {
            return w._oldFetch(input, init).then(function (response) {
                library._onHttpMonitor(response.url, response.status);
                return response;
            });
        };
    }
    library.start = function (keepAliveUrl) {
        library._keepAliveUrl = keepAliveUrl;
        library.timer = setInterval(function () {
            library._onTimer();
        }, 5000);
    };
    library.stop = function () {
        library._keepAliveUrl = null;
        if (self.timer) {
            clearInterval(_this.timer);
        }
    };

    library._onHttpMonitor = function (url, status) {
        var n = url.startsWith(window.location.origin);

        if (n === false) {
            w._keepAlive = true;
        }
    };
    library._onTimer = function () {
        if (w._keepAlive) {
            w._keepAlive = false;
            if (library._keepAliveUrl) {
                console.log("must keep alive");
                for (var i = 0, len = library._keepAliveUrl.length; i < len; i++) {
                    var uri = library._keepAliveUrl[i].KeepAliveUri;
                    w._oldFetch(uri, {
                        method: 'GET',
                        headers: { 'Content-Type': 'text/plain' }

                    }).then(function (res) {
                        console.log(res);
                    });
                }  
            }
        }
    };

    return library;
}();
