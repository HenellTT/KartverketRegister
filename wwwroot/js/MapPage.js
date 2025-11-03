
class MapFunctions {
    constructor(mapRef, ICONS) {
        this.map = mapRef;
        this.icons = ICONS;

        this.UpdatePosition = true;
        this.tempMarker = L.marker([0, 0], { draggable: true, icon: this.icons.Get["pin"] });
        this.vehiclePosition = L.marker([58.1608783456262, 7.9985872834629], { draggable: true, icon: this.icons.Get["helicopter"] });

        // Collection of all features (points and lines)
        this.features = [];

        // Toggle between placing points or drawing lines
        this.placeLine = false;
        this.currentLine = null;
    }

    Run() {
        this.map.on('click', (e) => this.handleMapClick(e));
        const btnHolder = document.querySelector('.leaflet-control-zoom');
        btnHolder.innerHTML += `<a class="leaflet-control-line" onclick='mf.toggleLineMode()' title="Line Mode" role="button" aria-label="Zoom out" aria-disabled="false"><span aria-hidden="true">/</span></a>`
        // Start vehicle tracking
        setTimeout(() => this.startVehicleTracking(), 200);
    }

    handleMapClick(e) {
        const { lat, lng } = e.latlng;

        if (this.placeLine) {
            // Drawing a line
            if (!this.currentLine) {
                this.currentLine = L.polyline([[lat, lng]], { color: 'blue' }).addTo(this.map);
                this.features.push({ type: 'LineString', latlngs: [[lat, lng]] });
            } else {
                this.currentLine.addLatLng([lat, lng]);
                this.features[this.features.length - 1].latlngs.push([lat, lng]);
            }
        } else {
            // Placing a point
            this.setMarkerPosition(lat, lng, this.icons.Get["pin"]);
            shlongPositionIntoForm(lat, lng);
            shlongHeightIntoForm(lat, lng);
            shlongGeoJsonIntoForm(JSON.stringify(this.toGeoJson()))

            this.features.push({
                type: 'Point',
                latlng: [lat, lng]
            });
        }
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
                    this.map.panTo([lat, lng], { animate: true, duration: 0.5 });
                }
            },
            (err) => console.error('Geolocation error:', err),
            {
                enableHighAccuracy: true,
                maximumAge: 1000,
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
            .bindPopup(`Lat: ${lat.toFixed(5)}, Lng: ${lng.toFixed(5)}`);
    }

    setMarkerPosition(lat, lng, icon = null) {
        if (icon) this.tempMarker.setIcon(icon);
        if (!this.map.hasLayer(this.tempMarker)) this.tempMarker.addTo(this.map);

        this.tempMarker.setLatLng([lat, lng])
            .bindPopup(`Lat: ${lat.toFixed(5)}, Lng: ${lng.toFixed(5)}`)
            .openPopup();
    }

    toggleLineMode() {
        if (this.placeLine == true) {
            this.placeLine = false;
            this.currentLine = null;
        } else {
            this.placeLine = true;
        }

    }

    // Export all features as GeoJSON
    toGeoJson() {
        const geoJson = {
            type: "FeatureCollection",
            features: this.features.map(f => {
                if (f.type === 'Point') {
                    return {
                        type: "Feature",
                        geometry: {
                            type: "Point",
                            coordinates: [f.latlng[1], f.latlng[0]] // GeoJSON uses [lng, lat]
                        },
                        properties: {}
                    };
                } else if (f.type === 'LineString') {
                    return {
                        type: "Feature",
                        geometry: {
                            type: "LineString",
                            coordinates: f.latlngs.map(ll => [ll[1], ll[0]])
                        },
                        properties: {}
                    };
                }
            })
        };
        return geoJson;
    }
}
const form = document.getElementById('tempMarkerFormForm');

async function shlongHeightIntoForm(lat,lng) {
    fetch(`/api/getHeight?lat=${lat}&lng=${lng}`).then((response) => { return response.json() }).then((data) => {
        console.log(data);
        form.height.value = data.height.toFixed(2);
    })
    
}
function shlongPositionIntoForm(lat, lng) {
    form.lng.value = lng;
    form.lat.value = lat;
}
function shlongGeoJsonIntoForm(GeoJson) {
    form.geojson.value = GeoJson;
}

form.addEventListener('submit', function (e) {
    e.preventDefault(); // Prevent page reload

    const form = e.target;
    const formData = new FormData(form); // Collect all form fields

    fetch('/Tempmarker/SubmitMarker', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) return response.text(); // or response.json() if returning JSON
            throw new Error('Network response was not ok');
        })
        .then(data => {
            document.getElementById('status').innerText = 'Marker saved successfully!';
            console.log('Server response:', data);
            form.reset(); // Optional: clear the form
        })
        .catch(error => {
            document.getElementById('status').innerText = 'Error saving marker.';
            console.error('Error:', error);
        });
    setTimeout(() => { document.getElementById('status').innerText = ''; }, 3000)

});


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
async function FetchaMahSubs() {
    try {
        const response = await fetch('/Marker/FetchMyMarkers');
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

function MyMarkerListElement(Marker) {
    const lat = Marker.lat;
    const lng = Marker.lng;
    return `<div class='tmk-cont'><p>type: ${Marker.type}<p>
                                  <p>Description: ${Marker.description}<p>
                                  <p>Lat: ${lat}<p>
                                  <p>Lng: ${lng}<p>
                                  <a href="/TempMarker/RegisterMarker?markerId=${Marker.markerId}"><button>Continue registration</button></a>
                                  <button onclick='DeleteMarkerReq(${Marker.markerId})'>Delete Marker</button>
                                  <button onclick='mf.setMarkerPosition(${lat},${lng}, mf.icons.Get["pin_crook"]); shlongPositionIntoForm(${lat},${lng}); mf.moveViewTo(${lat},${lng})'>Show on Map</button>
                                  </div>`;
}
function SubmissionElement(marker) {
    const {
        type,
        description,
        lat,
        lng,
        heightM,
        heightMOverSea,
        organization,
        accuracyM,
        obstacleCategory,
        isTemporary,
        expectedRemovalDate,
        lighting,
        source,
        userId,
        reviewedBy,
        reviewComment
    } = marker;

    return `
        <div class='tmk-cont'>
            <p><b>Type:</b> ${type || "N/A"}</p>
            <p><b>Description:</b> ${description || "N/A"}</p>
            <p><b>Organization:</b> ${organization || "N/A"}</p>
            <p><b>Height (m):</b> ${heightM ?? "N/A"}</p>
            <p><b>Height over sea (m):</b> ${heightMOverSea ?? "N/A"}</p>
            <p><b>Accuracy (m):</b> ${accuracyM ?? "N/A"}</p>
            <p><b>Obstacle category:</b> ${obstacleCategory || "N/A"}</p>
            <p><b>Temporary:</b> ${isTemporary ? "Yes" : "No"}</p>
            <p><b>Expected removal date:</b> ${expectedRemovalDate || "N/A"}</p>
            <p><b>Lighting:</b> ${lighting || "N/A"}</p>
            <p><b>Source:</b> ${source || "N/A"}</p>
            <p><b>Latitude:</b> ${lat}</p>
            <p><b>Longitude:</b> ${lng}</p>
            <p><b>User ID:</b> ${userId ?? "N/A"}</p>
            <p><b>Reviewed by:</b> ${reviewedBy ?? "N/A"}</p>
            <p><b>Review comment:</b> ${reviewComment || "N/A"}</p>

            <div class="tmk-actions">
                
                <button onclick='mf.setMarkerPosition(${lat}, ${lng}, mf.icons.Get["pin_crook"]);
                                  mf.moveViewTo(${lat}, ${lng});'>
                    Show on Map
                </button>
            </div>
        </div>`;
}

const mrkList = document.getElementById('mymrk');
const mrkListSub = document.getElementById('mymrksub');
function DeleteMarkerReq(markerId) {
    fetch(`/TempMarker/DeleteMarker?markerId=${markerId}`).then((resp) => { UpdateMyMarkerList() });
}

async function UpdateMyMarkerList() {
    const TempMarkers = await FetchaMahMarkah();
    mrkList.innerHTML = "";
    TempMarkers.forEach((Marker) => { mrkList.innerHTML += MyMarkerListElement(Marker); });
}
async function UpdateMySubmissionList() {
    const Submissions = await FetchaMahSubs();
    mrkListSub.innerHTML = "";
    Submissions.forEach((Marker) => { mrkListSub.innerHTML += SubmissionElement(Marker); });
}

const sideBarNavButtons = Array.from(document.getElementById('sideBarNav').children);

function SetActiveButton(button) {
    sideBarNavButtons.forEach((ibut) => { SetButtonState(ibut, false) })
    SetButtonState(button, true);
}
function SetButtonState(button, state) {
    if (state) {
        button.style.backgroundColor = "lightgrey";
    } else {
        button.style.backgroundColor = "white";
    }
}
