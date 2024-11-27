

window.onload = function () {

    // environment Config
    //var appDomain = 'http://192.168.1.105'
    var appDomain = 'https://www.occpy.com/'
    var el = document.getElementById("valid-span");
    if (el) {
        window.parent.postMessage("login", appDomain)
    } else {
        //window.parent.postMessage('error', 'http://192.168.1.105')
    }
}