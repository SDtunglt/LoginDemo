mergeInto(LibraryManager.library, {
    GetURLFromPage: function () {
        var returnStr = window.top.location.href;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        // console.log("GetURLFromPage: " + buffer);
        return buffer;
    },
    CopyClipboard: function (str) {
            var copyText = Pointer_stringify(str);
            const el = document.createElement('textarea');
            el.value = copyText;
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
        },
    
    HardReload: function () {
        window.indexedDB
            .databases()
            .then(function (r) {
                for (var i = 0; i < r.length; i++) {
                    window.indexedDB.deleteDatabase(r[i].name)
                }
                ;
            })
            .then(function () {
                if (window.confirm("Tải lại trang web?")) {
                    window.location.href = window.location.href.replace(/#.*$/, '');
                }
            });
    }
});