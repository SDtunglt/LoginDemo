mergeInto(LibraryManager.library, {
    FacebookLoginWeb: function () {
        console.log("Login Facekook");
        FB.login(function (response) {
            if (response.status == "connected") {
                FB.api(
                    "/me",
                    {
                        fields: "email,name,id,gender"
                    },
                    function (res) {
                        var data = JSON.stringify(res);
                        console.log("Respone Gender: " + data);
                        SendMessage("LoginPanel", "FacebookLoginResponse", data);
                    }
                );
            }
        }, {
            scope: 'public_profile,email'
        });
    },
       ShareGameFbWithApi: function (feedLink) {
            var fLink = Pointer_stringify(feedLink);
    
            console.log(fLink);
            FB.ui(
                {
                    method: "feed",
                    display: "popup",
                    link: fLink
                },
                function (response) {
                    if (response) {
                        var resData = JSON.stringify(response)
                        console.log(response)
                        SendMessage("GlobalDataManager", "HandleShareWeb", resData);
                    } else {
                        SendMessage("GlobalDataManager", "HandleShareWeb", "KO");
                    }
                });
        }
});