 // Assets/Pluging/JSUtils.jslib
 mergeInto(LibraryManager.library, {
   OpenURLInExternalWindow: function (url) {
     window.open(Pointer_stringify(url), "_blank");
   }
 });