mergeInto(LibraryManager.library, {
  registerVisibilityChangeEvent: function () {
    document.addEventListener("visibilitychange", function () {
      SendMessage("GamePlayScreen", "OnVisibilityChange", document.visibilityState);
    });
    if (document.visibilityState != "visible")
      SendMessage("GamePlayScreen", "OnVisibilityChange", document.visibilityState);
  }
});