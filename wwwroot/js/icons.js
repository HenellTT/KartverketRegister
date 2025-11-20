// If new icons are added, Just add them to this list;
const basicIcons = ["potato"];
// That's it, don't touch the rest it's very fragile 🥺
// Mucho Fragileses! ._.
const IconSize = 40;
class Icons {
    #icons
    constructor(IconList = basicIcons) {
        this.IconList = IconList;
        this.icons = {};

        IconList.forEach((icon) => {this.AddIcon(icon) });
    }
    get Get() {
        return this.icons;
    }
    AddIcon(IconName) {
        this.icons[IconName] = L.icon({
            iconUrl: `../img/icons/${IconName}.png`,
            iconSize: [IconSize, IconSize],
            iconAnchor: [IconSize/2, IconSize],
            popupAnchor: [0, -IconSize/2]
        });
    }
    GetIcons() {
        return this.IconList;
    }
}
