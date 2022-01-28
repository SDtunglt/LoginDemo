mergeInto(LibraryManager.library, {
    CheckXenData: function () {
        console.log("Get Data Xenforo");

        const gameChan = document.getElementById("unity-container");
        const uid = gameChan.getAttribute("data-uid") != null ? gameChan.getAttribute("data-uid") : "";
        const n = gameChan.getAttribute("data-name") != null ? gameChan.getAttribute("data-name") : "";
        const dataX = gameChan.getAttribute("data-x") != null ? gameChan.getAttribute("data-x") : "";
        const dataH = gameChan.getAttribute("data-h") != null ? gameChan.getAttribute("data-h") : "";
        const c = document.cookie;

        const obj = {
            id: uid,
            uname: n,
            cookie: c,
            x: dataX,
            h: dataH
        }

        SendMessage("Login", "XenResponse", JSON.stringify(obj));
    },
    CheckAppDesktop: function () {
        const gameChan = document.getElementById("unity-container");
        if (gameChan.hasAttribute("isDesktop")) {
            const attr = gameChan.getAttribute("isDesktop");
            console.log("isDesktop: " + attr);
            if (attr && attr == "true") {
                SendMessage("Login", "CheckAppResponse", attr);
            }
        }

        if (gameChan.hasAttribute("isFacebook")) {
            const isFb = gameChan.getAttribute("isFacebook");
            console.log("isFacebook: " + isFb);

            if (isFb && isFb == "true") {
                SendMessage("Login", "CheckAppFacebook", isFb);
            }
        }
    },

    IsFacebookInstantGame: function () {
        const gameChan = document.getElementById("unity-container");
        if (gameChan.hasAttribute("isFacebook")) {
            const isFb = gameChan.getAttribute("isFacebook");
            var temp = "false";
            if (isFb && isFb == "true") {
                temp = isFb;
            }
            var bufferSize = lengthBytesUTF8(temp) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(temp, buffer, bufferSize);
            return buffer;
        }

    },

    IsDesktop: function () {

        const gameChan = document.getElementById("unity-container");
        if (gameChan.hasAttribute("isDesktop")) {
            const isDesktop = gameChan.getAttribute("isDesktop");
            var temp = "false";
            if (isDesktop && isDesktop == "true") {
                temp = isDesktop;
            }
            var bufferSize = lengthBytesUTF8(temp) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(temp, buffer, bufferSize);
            return buffer;
        }
    },

    GetMqttConfig: function () {
        console.log("Get Mqtt");

        const gameChan = document.getElementById("unity-container");

        const dataX = gameChan.getAttribute("data-x") != null ? gameChan.getAttribute("data-x") : "";
        var bufferSize = lengthBytesUTF8(dataX) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(dataX, buffer, bufferSize);
        return buffer;
    },

    GetHostPortConfig: function () {
        console.log("Get HostPort");

        const gameChan = document.getElementById("unity-container");

        const dataX = gameChan.getAttribute("data-h") != null ? gameChan.getAttribute("data-h") : "";
        var bufferSize = lengthBytesUTF8(dataX) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(dataX, buffer, bufferSize);
        return buffer;
    },
});