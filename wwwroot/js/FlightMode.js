

class MapFunctions {
    constructor(mapRef, ICONS) {
        this.map = mapRef;

        this.icons = ICONS;

        this.UpdatePosition = true;
        this.tempMarker = L.marker([0, 0], { draggable: true, icon: this.icons.Get["pin"] });
        this.vehiclePosition = L.marker([0, 0], { draggable: true, icon: this.icons.Get["helicopter"] });
    }

    Run() {
        this.map.on('click', async (e) => {
            const lat = e.latlng.lat;
            const lng = e.latlng.lng;
            //console.log(`lat:${lat}, lng:${lng}`);

            // Move marker to clicked location
            this.setMarkerPosition(lat, lng, mf.icons.Get["pin"]);
            shlongPositionIntoForm(lat, lng);
            shlongHeightIntoForm(lat, lng);
        });
        // Init shit that has to be done fr fr
        this.map.setZoom(17)
        this.map.setView([58.1637, 8.0031])
        setTimeout(this.startVehicleTracking(), 200);
    }

    startVehicleTracking() {
        if (!navigator.geolocation) {
            console.error('Geolocation is not supported by this browser.');
            return;
        }

        navigator.geolocation.watchPosition(
            (position) => {
                if (this.UpdatePosition) {
                    const lat = position.coords.latitude;
                    const lng = position.coords.longitude;
                    this.setVehiclePosition(lat, lng);

                    // Pan map smoothly to current position
                    this.map.panTo([lat, lng], { animate: true, duration: 0.5 });
                }

            },
            (err) => {
                console.error('Geolocation error:', err);
            },
            {
                enableHighAccuracy: true,
                maximumAge: 1000,   // use cached location up to 1 second
                timeout: 5000
            }
        );
    }
    moveViewTo(lat, lng) {
        this.map.panTo([lat, lng], { animate: true, duration: 0.5 });
    }

    setVehiclePosition(lat, lng) {
        if (!this.map.hasLayer(this.vehiclePosition)) {
            this.vehiclePosition.addTo(this.map);
        }
        this.vehiclePosition.setLatLng([lat, lng])
            .bindPopup(`Lat: ${lat.toFixed(5)}, Lng: ${lng.toFixed(5)}`)
        //.openPopup();
    }

    // Set the temp marker to the desired position
    setMarkerPosition(lat, lng, icon = null) {
        if (icon != null) {
            this.tempMarker.setIcon(icon);
        }
        if (!this.map.hasLayer(this.tempMarker)) {
            this.tempMarker.addTo(this.map);
        }
        this.tempMarker.setLatLng([lat, lng])
            .bindPopup(`Lat: ${lat.toFixed(5)}, Lng: ${lng.toFixed(5)}`)
            .openPopup();


    }

}



async function FetchaMahMarkah() {
    try {
        const response = await fetch('/Tempmarker/FetchMyMarkers');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Fetch error:', error);
        return null;
    }
}

