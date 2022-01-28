var FullScreenRequest = {
    FullScreenRequest: function () {
        document.makeFullscreen('unity-container');
    },

    CloseFullscreen: function () {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.webkitExitFullscreen) { /* Safari */
            document.webkitExitFullscreen();
        } else if (document.msExitFullscreen) { /* IE11 */
            document.msExitFullscreen();
        }
    }
};
mergeInto(LibraryManager.library, FullScreenRequest);