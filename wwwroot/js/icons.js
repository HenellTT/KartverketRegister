// If new icons are added, Just add them to this list;
const basicIcons = ["potato", "user_icons"];
// That's it, don't touch the rest it's very fragile 🥺
// Mucho Fragileses! ._.
class Icons {
    #icons
    constructor(IconList = basicIcons) {
        this.IconList = IconList;
        this.#icons = {};

        IconList.forEach((icon) => {this.AddIcon(icon) });
    }
    get Get() {
        return this.#icons;
    }
    AddIcon(IconName) {
        this.#icons[IconName] = L.icon({
            iconUrl: `../img/icons/${IconName}.png`,
            iconSize: [38, 38],
            iconAnchor: [19, 38],
            popupAnchor: [0, -38]
        });
    }
    GetIcons() {
        return this.IconList;
    }
}