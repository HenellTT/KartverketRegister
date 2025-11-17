class PilotFlightMode {
    constructor(map, L) {
        // Injected instances
        this.map = map;
        this.L = L;
        this.Icons = Window.icons;

        // Actions
        this.ActionList = [];     // stores Action objects
        this.ActionEntries = [];  // stores Leaflet layers for map display
        this.groupLayer = this.L.layerGroup().addTo(this.map);

        // Clickies
        this.Mode = "Marker";
        this._lastClick = null;
        
    }

    Init() {
        // Optional initialization logic
        console.log("PilotFlightMode initialized");
        this._lastClick = null;

        this.map.on('click', (event) => {
            const cords = new Cords(event.latlng.lat, event.latlng.lng);
            console.log(cords);
            if (this.Mode === "Marker") {
                this.AddMarker(cords);
            }

            if (this.Mode === "Line") {
                // First click → store starting point
                if (!this._lastClick) {
                    this._lastClick = cords;
                    return;
                }

                // Second click → create line
                this.AddLine(this._lastClick, cords);

                // Reset for next line segment
                this._lastClick = cords;
            }
            this.ActionList[this.ActionList.length - 1].SetHeight();
        });
    }
    SetMode(mode) {
        this.Mode = mode;
    }

    RevertLastAction() {
        if (this.ActionList.length === 0) return;

        // Remove last action layer from map
        const lastLayer = this.ActionEntries.pop();
        this.groupLayer.removeLayer(lastLayer);

        // Remove from ActionList
        this.ActionList.pop();
    }

    CreateGroupLayer() {
        // Create a new layer group for current actions
        this.groupLayer = this.L.layerGroup().addTo(this.map);

        // Add existing actions to the new group
        this.ActionEntries.forEach(layer => this.groupLayer.addLayer(layer));
    }

    AddMarker(cords, icon = this.Icons.Get["l_pin_yellow"]) {
        const action = new ActionMarker(cords, icon);
        this.ActionList.push(action);

        const marker = this.L.marker([cords.lat, cords.lng], { icon: action.icon });
        marker.addTo(this.groupLayer);
        this.ActionEntries.push(marker);
    }

    AddLine(fromCords, toCords) {
        const action = new ActionLine(fromCords, toCords);
        this.ActionList.push(action);

        const polyline = this.L.polyline(
            [[fromCords.lat, fromCords.lng], [toCords.lat, toCords.lng]],
            { color: 'green' }   // ← changed here
        );

        polyline.addTo(this.groupLayer);
        this.ActionEntries.push(polyline);
    }
    GetHeight() {
        let h = 0;
        for (let Action in this.ActionList) {
            let ActionObj = this.ActionList[Action];
            if (ActionObj.height != null) return ActionObj.height;
        }
        return h;
    }

    ExportGeoJson() {
        const features = [];

        this.ActionList.forEach(action => {
            if (action instanceof ActionMarker) {
                features.push({
                    type: "Feature",
                    geometry: {
                        type: "Point",
                        coordinates: [action.lng, action.lat]
                    },
                    properties: {}
                });
            } else if (action instanceof ActionLine) {
                features.push({
                    type: "Feature",
                    geometry: {
                        type: "LineString",
                        coordinates: [
                            [action.lng, action.lat],
                            [action.toCords.lng, action.toCords.lat]
                        ]
                    },
                    properties: {}
                });
            }
        });

        const geojson = {
            type: "FeatureCollection",
            features: features
        };

        console.log(geojson);
        return geojson;
    }
    ExportGeoJsonString() {
        return JSON.stringify(this.ExportGeoJson());
    }
    ClearActions() {
        this.ActionList = [];
        this.ActionEntries = [];
        this.groupLayer.clearLayers();
    }
}

class Cords {
    constructor(lat, lng) {
        this.lat = lat;
        this.lng = lng;
    }
}

class ActionBase {
    constructor(baseCords) {
        this.lat = baseCords.lat;
        this.lng = baseCords.lng;
        this.height = 0;
    }
    async SetHeight() {
        const resp = await fetch(`/api/getHeight?lat=${this.lat}&lng=${this.lng}`);
        const data = await resp.json();
        console.log(data);
        this.height = data.height;
    }

}

class ActionMarker extends ActionBase {
    constructor(baseCords, icon) {
        super(baseCords);
        this.icon = icon;
    }
}

class ActionLine extends ActionBase {
    constructor(baseCords, toCords) {
        super(baseCords);
        this.toCords = toCords;
    }
}

function SubmitReview(PFM) {
    let desc = prompt("Describe the obstacle / Note for yourself");

}
