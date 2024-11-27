
window.onload = function () {

    var appLoadingEl = document.getElementById('app-loading');
    var accountCardEl = document.getElementById('account-card');

    // environment Config
    //var appDomain = 'http://192.168.1.105'
    var appDomain = 'https://www.occpy.com'
        setTimeout(() => {
            if (window.top[0] !== undefined) {
                window.top.postMessage("height=" + window.document.body.scrollHeight, appDomain)
            }
        }, 200)


        var emailEl = document.getElementById('email');
        if (emailEl) {
            emailEl.focus();

            emailEl.addEventListener('keyup', emailEl => {
                var errorCred = document.getElementById('error-cred');

                if (errorCred) {
                    errorCred.hidden = true;
                }
            })
        }

        var el1 = document.getElementById("login-button");
        if (el1) {
            el1.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("cancel", appDomain)
                }
            })
        }

        var el2 = document.getElementById("cancel-button");
        if (el2) {
            el2.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("cancel", appDomain)
                }
            })
        }

        var el3 = document.getElementById("forget-button");
        if (el3) {
            el3.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("forget", appDomain)
                }
            })
        }

        var el4 = document.getElementById("create-button");
        if (el4) {
            el4.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("create", appDomain)
                }
            })
        }

        var googleButt = document.getElementById("Google");
        if (googleButt) {
            googleButt.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("Google", appDomain)
                }
            })
        }
        var fbButt = document.getElementById("Facebook");
        if (fbButt) {
            fbButt.addEventListener("click", function () {
                setLoader()
                if (window.top[0] !== undefined) {
                    window.top.postMessage("Facebook", appDomain)
                }
            })
        }


    function setLoader() {
        if (appLoadingEl && appLoadingEl.style) {
            appLoadingEl.style.display = 'flex';
        }
        if (accountCardEl && accountCardEl.style) {
            accountCardEl.style.display = 'none';
        }
        
    }
}
