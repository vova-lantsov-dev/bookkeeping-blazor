window.Extensions = {
    /**
     * @return {string}
     */
    ReadStorage: function (key) {
        return localStorage.getItem(key);
    },
    WriteStorage: function (key, value) {
        localStorage.setItem(key, value);
    },
    RemoveStorage: function (key) {
        localStorage.removeItem(key);
    }
};

function triggerNavBar() {
    const x = document.getElementById("nav-bar");
    if (x.className.indexOf("w3-show") === -1) {
        x.className += " w3-show";
    } else {
        x.className = x.className.replace(" w3-show", "");
    }
}