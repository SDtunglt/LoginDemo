var ExternalWindow = {
    OpenURLInExternalWindow: function (url) {
        console.log("URL: " + Pointer_stringify(url));
        window.open(Pointer_stringify(url), "_blank");
    }
}
mergeInto(LibraryManager.library, ExternalWindow);