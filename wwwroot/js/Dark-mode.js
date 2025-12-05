function dms(bool = "wetSocks") {
    let body = document.querySelector('body');
    let mappyUwU = document.getElementById("map");

    if (bool == "wetSocks") {
        if (body.classList.contains("MayDarknessSwallowYourSoul")) {
            bool = false;
        } else {
            bool = true;
        }
    }

    if (bool) {
        sessionStorage.setItem('DarkMode', true);
        body.classList.add("MayDarknessSwallowYourSoul");
        mappyUwU?.classList.add("MayDarknessSwallowYourSoulMap");
    } else {
        sessionStorage.setItem('DarkMode', false);
        body.classList.remove("MayDarknessSwallowYourSoul");
        mappyUwU?.classList.remove("MayDarknessSwallowYourSoulMap");
    }
}