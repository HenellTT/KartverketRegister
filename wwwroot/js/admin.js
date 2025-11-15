
async function FetchMarkers(status) {
    try {

        let response = await fetch('/Superadmin/FetchAllMarkers?markerStatus=' + status);
        
        if (!response.ok || response.redirected) {
            response = await fetch('/Admin/GetAllMarkers?markerStatus=' + status);
            if (!response.ok) {
                throw new Error("fuck");
            }
        }

        const data = await response.json();
        return data.success ? data.markers : [];
    } catch (error) {
        console.error('Error:', error);
        return [];
    }
}

const mymrk = document.getElementById('mymrk');

async function UpdateMarkerList(status) {
    let markers = await FetchMarkers(status);
    
    let stringToAdd = "";
    markers.forEach((mrk) => { stringToAdd += CreateMarkerElement(mrk)})
    mymrk.innerHTML = stringToAdd;
} 
async function UpdateMarkerListTable(status) {
    let markers = await FetchMarkers(status);

    let stringToAdd = `<table><tr">
        <td>Type</td>
        <td><u>User</u></td>
        <td>Height</td>
        <td>Height(sea)</td>
        <td>Organization</td>
        <td>Accuracy</td>
        <td>Category</td>
        <td>Status</td>
        <td>Actions</td>
    </tr>`;
       
    markers.forEach((mrk) => { stringToAdd += CreateMarkerRow(mrk) })
    mymrk.innerHTML = stringToAdd + "</table>";
} 

function CreateMarkerElement(marker) {
    return `
    <div class="tmk-cont trw-${marker.state}" id='markerBox-${marker.markerId}' style="padding:10px; border:1px solid #ccc; border-radius:8px; margin-bottom:10px;">
        <h3>${marker.type}</h3>
        <div class="marker-info" style="display:flex; flex-wrap:wrap; gap:10px;">
            <p><b>Description:</b> ${marker.description}</p>
            <p><b>By:</b> <u>${marker.userName}</u></p>
            <p><b>Height (M):</b> ${marker.heightM ?? 'N/A'}</p>
            <p><b>Height Over Sea (M):</b> ${marker.heightMOverSea ?? 'N/A'}</p>
            <p><b>Organization:</b> ${marker.organization ?? 'N/A'}</p>
            <p><b>Accuracy (M):</b> ${marker.accuracyM ?? 'N/A'}</p>
            <p><b>Obstacle Category:</b> ${marker.obstacleCategory ?? 'N/A'}</p>

            <p><b>Review Comment:</b> ${marker.reviewComment ?? 'N/A'}</p>

        </div>
        <button onclick="postRedirect('/Admin/Review', { markerId: ${marker.markerId} })" style="margin-top:10px;">Review</button>
        <button onclick="DeleteMarker(${marker.markerId})">Delete</button>
    </div>
    `;
}
function CreateMarkerRow(marker) {
    return `
    <tr id="markerRow-${marker.markerId}" class="trw-${marker.state}">
        <td>${marker.type}</td>
        <td><u>${marker.userName}</u></td>
        <td>${marker.heightM ?? 'N/A'}</td>
        <td>${marker.heightMOverSea ?? 'N/A'}</td>
        <td>${marker.organization ?? 'N/A'}</td>
        <td>${marker.accuracyM ?? 'N/A'}</td>
        <td>${marker.obstacleCategory ?? 'N/A'}</td>
        <td>${marker.state ?? 'N/A'}</td>
        <td style="text-align:right;">
            <button onclick="postRedirect('/Admin/Review', { markerId: ${marker.markerId} })">Review</button>
            <button onclick="DeleteMarker(${marker.markerId})">Delete</button>
        </td>
    </tr>
    `;
}
const sts = document.getElementById("stateToSee");
function DeleteMarker(MarkerId) {
    fetch(`./Admin/DeleteMarker?MarkerId=${MarkerId}`).then(() => {
        UpdateMarkerListTable(sts.value);
    })
    
}
function postRedirect(url, params = { }) {
    // Create a hidden form
    const form = document.createElement('form');
    form.method = 'POST';
    form.action = url;

    // Add params as hidden inputs
    for (const key in params) {
        if (params.hasOwnProperty(key)) {
            const input = document.createElement('input');
    input.type = 'hidden';
    input.name = key;
    input.value = params[key];
    form.appendChild(input);
        }
    }

    document.body.appendChild(form);
    form.submit();
}
function SmoothScroll(topPx) {
    window.scrollTo({
        top: -2000000000,
        left: 0,
        behavior: 'smooth'
    });
    window.scrollTo({
        top: topPx,
        left: 0,
        behavior: 'smooth' 
    });
}
function scrollToMarker(markerId) {
    const topsset = document.getElementById(`markerBox-${markerId}`).offsetTop;
    SmoothScroll(topsset);
}

class OverComplicatedAverage {
    constructor() {
        this.numbers = [];
    }
    average() {
        let sum = 0;
        this.numbers.forEach(n => sum += n)
        return sum / this.numbers.length;
    }
    addNumber(n) {
        this.numbers.push(n);
    }
}

function ResizeIcons(size, ICONS) {
    const keys = Object.keys(ICONS.icons);
    keys.forEach((key) => {
        iconOptions = ICONS.icons[key].options;
        iconOptions['iconAnchor'] = [size / 2, size];
        iconOptions['iconSize'] = [size, size];
        iconOptions['popupAnchor'] = [0, -size];

    })
}

async function UpdateMapMarkers(map, L, type = 'Everything') {
    const markers = await FetchMarkers(type);
    if (markers.length == 0) {
        return "brr";
    }
    map.markers = map.markers || [];

    // kill old markers
    map.markers.forEach(m => map.removeLayer(m));
    map.markers = [];

    let ocaLat = new OverComplicatedAverage();
    let ocaLng = new OverComplicatedAverage();

    // Add new markers
    markers.forEach((markerData) => {
        const iconName = markerData.obstacleCategory.toLowerCase().replaceAll(" ", "");
        const icon = map.ICONS.Get[iconName]; // fixed: use map.ICONS, not this.ICONS

        const mrk = L.marker([markerData.lat, markerData.lng], {
            draggable: false,
            icon: icon
        }).bindPopup(`<div onclick='scrollToMarker(${markerData.markerId})'>${markerData.type}<br>${markerData.description}<br>
        <button onclick="postRedirect('/Admin/Review', { markerId: ${markerData.markerId} })" style="margin-top:10px;">Review</button>
        </div>`).on('click', () => {
            scrollToMarker(markerData.markerId);
        });

        mrk.addTo(map);
        map.markers.push(mrk);

        ocaLat.addNumber(markerData.lat)
        ocaLng.addNumber(markerData.lng)

    });
    
    const AvgLat = ocaLat.average();
    const AvgLng = ocaLng.average();
    //map.setView([AvgLat, AvgLng], 10);

    const bounds = L.latLngBounds(markers.map(m => [m.lat, m.lng]));
    map.fitBounds(bounds, { padding: [50, 50] });
}