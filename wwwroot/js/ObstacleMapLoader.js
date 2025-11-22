class ObstacleMapLoader {
    constructor(map, L) {
        this.L = L;
        this.map = map;
    }
    async Init() {
        const MarkerResponse = await fetch('/Marker/GetObstacles');
        const JsonizedResponse = await MarkerResponse.json();

        if (!JsonizedResponse.success) {
            return false;
        } 
        const Markers = JsonizedResponse.data;

        Markers.forEach((mrk) => {
            let iconName = mrk.obstacleCategory.toLowerCase().replaceAll(" ", "");
            let icon = Window.icons.Get[iconName];

            if (mrk.geoJson) {
                mrk.geoJson = JSON.parse(mrk.geoJson);
                this.L.geoJSON(mrk.geoJson, {
                    pointToLayer: (feature, latlng) => {
                        return this.L.marker(latlng, {
                            icon: icon,
                            draggable: false
                        });
                    }
                }).addTo(this.map);
            }
        }) //

        
    }
}