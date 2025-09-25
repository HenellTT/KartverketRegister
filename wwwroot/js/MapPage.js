

class MapFunctions {
    constructor(mapRef,ICONS) {
        this.map = mapRef;

        this.icons = ICONS;

        this.UpdatePosition = true;
        this.tempMarker = L.marker([0, 0], { draggable: true, icon: this.icons.Get["pin"] });
        this.vehiclePosition = L.marker([58.1608783456262, 7.9985872834629], { draggable: true, icon: this.icons.Get["helicopter"] });
    }

    Run() {
        this.map.on('click', (e) => {
            const lat = e.latlng.lat;
            const lng = e.latlng.lng;
            //console.log(`lat:${lat}, lng:${lng}`);

            // Move marker to clicked location
            this.setMarkerPosition(lat, lng, mf.icons.Get["pin"]);
            shlongPositionIntoForm(lat, lng);
        });
        // Init shit that has to be done fr fr
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
    moveViewTo(lat,lng) {
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
const form = document.getElementById('tempMarkerFormForm');

function shlongPositionIntoForm(lat, lng) {
    form.lng.value = lng;
    form.lat.value = lat;
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


const mrkList = document.getElementById('mymrk');
function DeleteMarkerReq(markerId) {
    fetch(`/TempMarker/DeleteMarker?markerId=${markerId}`).then((resp) => { UpdateMyMarkerList() });
}

async function UpdateMyMarkerList() {
    const TempMarkers = await FetchaMahMarkah();
    mrkList.innerHTML = "";
    TempMarkers.forEach((Marker) => { mrkList.innerHTML += MyMarkerListElement(Marker); });
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
