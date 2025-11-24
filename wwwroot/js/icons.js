// If new icons are added, Just add them to this list;
const basicIcons = {icons: ["potato"], gifs:[]};
// That's it, don't touch the rest it's very fragile 🥺
// Mucho Fragileses! ._.
const IconSize = 40;
let a;
class Icons {
    #icons
    constructor(IconList = basicIcons) {
        
        this.IconList = Array.from(IconList.icons);
        this.GifList = Array.from(IconList.gifs);
        this.icons = {};

        this.IconList.forEach((icon) => { this.AddIcon(icon) });
        this.GifList.forEach((icon) => { this.AddIcon(icon, "gif")});

    }
    get Get() {
        return this.icons;
    }
    AddIcon(IconName, FileEx = "png") {
        let DName;
        if (FileEx != "png") {
            DName = `${IconName}_${FileEx}`;
        } else {
            DName = IconName
        }
        this.icons[DName] = L.icon({
            iconUrl: `../img/icons/${IconName}.${FileEx}`,
            iconSize: [IconSize, IconSize],
            iconAnchor: [IconSize/2, IconSize],
            popupAnchor: [0, -IconSize/2]
        });
    }
    GetIcons() {
        return this.IconList;
    }
}
