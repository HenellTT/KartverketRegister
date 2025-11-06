class MapMarkers {
    constructor(mapRef, L, ICONS) {
        this.map = mapRef;
        this.L = L;
        this.icons = ICONS;
        this.Markers = []; // store leaflet markers
    }

    async Obstaculator(posModel) {
        const Lat = posModel.lat;
        const Lng = posModel.lng;
        console.log(`[Obstacle] lat: ${Lat} lng: ${Lng} `);

        // Fetch obstacles from server
        const response = await fetch("/Marker/GetObstacles", {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Lat, Lng })
        });

        const markersData = await response.json();
        console.log(`[Obstacle] Markers: ${markersData}`);
        // Clear existing markers
        this.clearMarkers();

        // Create new markers
        markersData.forEach(markerData => {
            const iconName = markerData.obstacleCategory.toLowerCase().replaceAll(" ", "");
            const icon = this.icons.Get[iconName]; // assumes you have pre-defined icons

            const mrk = this.L.marker([markerData.lat, markerData.lng], {
                draggable: false,
                icon: icon
            }).bindPopup(`
                <div onclick='scrollToMarker(${markerData.markerId})'>
                    ${markerData.type}<br>
                    ${markerData.description}<br>
                </div>
            `);

            mrk.addTo(this.map);       // add to Leaflet map
            this.Markers.push(mrk);     // keep reference
        });
    }

    // Optional: clear all markers from map
    clearMarkers() {
        this.Markers.forEach(m => this.map.removeLayer(m));
        this.Markers = [];
    }
}
