
const form = document.querySelector("form");
const outputField = document.getElementById('returnMsg');
const submitButton = document.querySelector('button[type="submit"]');


form.addEventListener("submit", async (event) => {
    event.preventDefault(); // Stop normal form submission
    const data = Object.fromEntries(new FormData(form).entries());

    // Convert number fields properly
    const numericFields = ["Lat", "Lng", "HeightM", "HeightMOverSea", "AccuracyM"];
    numericFields.forEach(f => data[f] = data[f] ? parseFloat(data[f]) : null);

    // Convert checkbox
    data.IsTemporary = form.querySelector("[name='IsTemporary']").checked;
    data.TempMarkerId = location.toLocaleString().split('=')[1];
    // Optional date (may be empty)
    if (data.ExpectedRemovalDate === "") data.ExpectedRemovalDate = null;

    const response = await fetch(form.action, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    const reply = await response.json();
    const outputField = document.getElementById("returnMsg");
    outputField.innerHTML = reply.message;

    if (reply.success) {
        outputField.style.color = "green";
        SuccessfulReq();
    } else {
        outputField.style.color = "red";
        FailedReq();
    }
});


function SuccessfulReq(msg) {
    submitButton.disabled = true;
    outputField.innerHTML += " - Redirecting to back to home . . .";
    let params = new URLSearchParams(document.location.search)
    let markerIdToDelete = params.get("markerId");
    fetch(`/TempMarker/DeleteMarker?markerId=${markerIdToDelete}`);

    setTimeout(() => {
        location.href = '/';
    }, 1500)
}
function FailedReq() {

}

function ValidateForm(formeeData) {
    FieldsToBeVerified = ['Type','Description','Lat','Lng','HeightM','HeightMOverSea','Organization','AccuracyM','ObstacleCategory','Source'];

    for (const item of formeeData) {
        console.log(item);
        if (item[1] === '' && FieldsToBeVerified.includes(item[0])) return false;
    }
    return true;
}
