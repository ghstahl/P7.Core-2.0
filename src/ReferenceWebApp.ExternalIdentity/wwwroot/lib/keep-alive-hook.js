var keepAliveHook = (function () {
    var library = {};
    let w = window;
    w._keepAlive = false;

    w._oldOpen = XMLHttpRequest.prototype.open;
    let onStateChange = (event) => {
        if (event.currentTarget.readyState === 4) {
            library._onHttpMonitor(event.currentTarget.responseURL, event.currentTarget.status);
        }
    };
    XMLHttpRequest.prototype.open = () => {
        // when an XHR object is opened, add a listener for its readystatechange events
        this.addEventListener('readystatechange', onStateChange);
        // run the real `open`
        w._oldOpen.apply(this, arguments);
    };
    if (!window.fetch.polyfill) {
        w._oldFetch = w.fetch;

        window.fetch = (input, init) => {
            return w._oldFetch(input, init).then(response => {
                library._onHttpMonitor(response.url, response.status);
                return response;
            });
        };
    }
    library.start = (keepAliveUrl) => {
        library._keepAliveUrl = keepAliveUrl;
        library.timer = setInterval(() => {
            library._onTimer();
        }, 5000);

    }
    library.stop = () => {
        library._keepAliveUrl = null;
        if (self.timer) {
            clearInterval(this.timer);
        }
    }
    
    library._onHttpMonitor = (url, status) =>{
        let n = url.startsWith(window.location.origin);

        if (n === false) {
            w._keepAlive = true;
        }
    }
    library._onTimer=()=> {
        if (w._keepAlive) {
            w._keepAlive = false;
            if (library._keepAliveUrl) {
                console.log("must keep alive")
                fetch(library._keepAliveUrl,
                        {
                            method: 'GET',
                            headers: { 'Content-Type': 'text/plain' },

                        })
                    .then(res => res.json())
                    .then(function (res) {
                        console.log(res);
                    });
            }
        }
    }
   
    return library;
})();
 
