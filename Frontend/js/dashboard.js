let xhr = new XMLHttpRequest();
xhr.addEventListener("readystatechange", function () {
    if (this.readyState === 4) {
        if (this.status == 200) {
            console.log(this.responseText);
        } else {
            window.location.replace("http://hec.zirk.eu/");
        }
    }
});
xhr.open("POST", "http://93.118.34.39:5151/devices");
xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
xhr.send("token=" + sessionStorage['token']);