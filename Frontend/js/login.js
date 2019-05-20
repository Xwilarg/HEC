let xhr = new XMLHttpRequest();
xhr.addEventListener("readystatechange", function () {
    if (this.readyState === 4) {
        if (this.status == 204) {
            window.location.replace("http://hec.zirk.eu/dashboard.html");
        }
    }
});
xhr.open("POST", "http://93.118.34.39:5151/token");
xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
xhr.send("token=" + sessionStorage['token']);

function login() {
    let xhr = new XMLHttpRequest();
    xhr.addEventListener("readystatechange", function () {
        if (this.readyState === 4) {
            if (this.status == 200) {
                sessionStorage['token'] = JSON.parse(this.responseText).userToken;
                window.location.replace("http://hec.zirk.eu/dashboard.html");
            } else {
                var now     = new Date(); 
                var hour    = now.getHours();
                var minute  = now.getMinutes();
                var second  = now.getSeconds();   
                if(hour.toString().length == 1)
                    hour = '0' + hour;
                if(minute.toString().length == 1)
                    minute = '0' + minute;
                if(second.toString().length == 1)
                    second = '0' + second;
                if (this.status === 401) {
                    document.getElementById("reply").innerHTML = hour + ":" + minute + ":" + second + ": Username and password doesn't match.";
                } else {
                    document.getElementById("reply").innerHTML = hour + ":" + minute + ":" + second + ": The server isn't available, please retry later.";
                }
            }
        }
    });
    xhr.open("POST", "http://93.118.34.39:5151/auth");
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    xhr.send("username=" + document.getElementById("username").value + "&password=" + CryptoJS.SHA1(document.getElementById("password").value));
}