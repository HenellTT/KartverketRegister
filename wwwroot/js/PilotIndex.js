async function FetchaMahMarkah() {
    try {
        const response = await fetch('/Tempmarker/FetchMyMarkers');
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    } catch (error) {
        console.error('Fetch error:', error);
        return null;
    }
}

async function FetchaMahSubs() {
    try {
        const response = await fetch('/Marker/FetchMyMarkers');
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    } catch (error) {
        console.error('Fetch error:', error);
        return null;
    }
}

function MyMarkerListElement(Marker) {
    const { markerId, type, description, lat, lng } = Marker;
    return `
        <tr>
            <td>${type}</td>
            <td>${description}</td>
            <td>${lat.toFixed(3)}</td>
            <td>${lng.toFixed(3) }</td>
            <td>
                <a href="/TempMarker/RegisterMarker?markerId=${markerId}">
                    <button>Continue registration</button>
                </a>
                <button onclick='DeleteMarkerReq(${markerId})'>Delete</button>
                
            </td>
        </tr>`;
}

function SubmissionElement(marker) {
    const {
        type, description, lat, lng, heightM, heightMOverSea,
        organization, accuracyM, obstacleCategory, isTemporary,
        expectedRemovalDate, lighting, source, userId,
        reviewedBy, reviewComment
    } = marker;

    return `
        <tr>
            <td>${type || "N/A"}</td>
            <td>${description || "N/A"}</td>
            <td>${heightM ?? "N/A"}</td>
            <td>${heightMOverSea ?? "N/A"}</td>    
            <td>${obstacleCategory || "N/A"}</td>
            <td>${reviewedBy ?? "N/A"}</td>
            <td>${reviewComment || "N/A"}</td>
            <td>
                <button onclick='location.href = "viewmarker/${marker.markerId}"'>
                    Full info
                </button>
            </td>
        </tr>`;
}

const mrkList = document.getElementById('mymrk');
const mrkListSub = document.getElementById('mymrksub');

function DeleteMarkerReq(markerId) {
    fetch(`/TempMarker/DeleteMarker?markerId=${markerId}`)
        .then(() => UpdateMyMarkerList());
}

async function UpdateMyMarkerList() {
    const TempMarkers = await FetchaMahMarkah();
    if (!TempMarkers) return;
    mrkList.innerHTML = `
        <table border="1">
            <thead>
                <tr>
                    <th>Type</th>
                    <th>Description</th>
                    <th>Latitude</th>
                    <th>Longitude</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${TempMarkers.map(MyMarkerListElement).join('')}
            </tbody>
        </table>`;
}

async function UpdateMySubmissionList() {
    const Submissions = await FetchaMahSubs();
    if (!Submissions) return;
    mrkListSub.innerHTML = `
        <table border="1">
            <thead>
                <tr>
                    <th>Type</th>
                    <th>Description</th>
                    <th>Height (m)</th>
                    <th>Height over sea (m)</th>
                    <th>Obstacle Category</th>
                    
                    <th>Reviewed By</th>
                    <th>Review Comment</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${Submissions.map(SubmissionElement).join('')}
            </tbody>
        </table>`;
}
