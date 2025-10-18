
async function FetchMarkers(status) {
    try {
        const response = await fetch('/Admin/GetAllMarkers?markerStatus=' + status);
        if (!response.ok) throw new Error('Network response was not ok');

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

function CreateMarkerElement(marker) {
    return `
    <div class="tmk-cont trw-${marker.state}" style="padding:10px; border:1px solid #ccc; border-radius:8px; margin-bottom:10px;">
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
const sts = document.getElementById("stateToSee");
function DeleteMarker(MarkerId) {
    fetch(`./Admin/DeleteMarker?MarkerId=${MarkerId}`).then(() => {
        UpdateMarkerList(sts.value);
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