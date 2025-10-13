
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
    <div class="tmk-cont">
        <h3>${marker.type}</h3>
        <p><b>Description:</b>${marker.description}</p>
        <p>By: <u>${marker.userId}</u></p>
        <button>Details</button>
        <button>Reject</button>
        <button>Approve</button>
    </div>
    `;
}