var FirebaseLog = {
    FirebaseLogEvent: function (eventName) {
        var name = Pointer_stringify(eventName);
        const analytics = firebase.analytics();
        analytics.logEvent("" + name);
    },

    FirebaseLogEventWithParam: function (eventName, paramName, paramValue) {
        var name = Pointer_stringify(eventName);
        var pName = Pointer_stringify(paramName);
        var pValue = Pointer_stringify(paramValue);
        const pNames = pName.split("|");
        const pValues = pValue.split("|");
        const analytics = firebase.analytics();
        var param = {};
        for (var i = 0; i < pNames.length; i++) {
            param[pNames[i]] = pValues[i];
        }
        analytics.logEvent("" + name, param);
    }
    
}
mergeInto(LibraryManager.library, FirebaseLog);