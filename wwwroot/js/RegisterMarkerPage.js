const form = document.querySelector("form");
const outputField = document.getElementById('returnMsg');
const submitButton = document.querySelector('button[type="submit"]');

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const formData = new FormData(form);
    
    // Valider at obligatoriske felt er fylt ut
    if (!ValidateForm(formData)) {
        outputField.innerHTML = "Please fill in all required fields.";
        outputField.style.color = "red";
        return;
    }
    
    // Legg til TempMarkerId fra URL
    const params = new URLSearchParams(window.location.search);
    formData.set('TempMarkerId', params.get('markerId') || '');
    
    // Håndter checkbox (unchecked sender ikke verdi)
    if (!form.querySelector("[name='IsTemporary']").checked) {
        formData.set('IsTemporary', 'false');
    }

    const response = await fetch(form.action, {
        method: "POST",
        body: formData
    });

    const reply = await response.json();
    outputField.innerHTML = reply.message;

    if (reply.success) {
        outputField.style.color = "green";
        SuccessfulReq();
    } else {
        outputField.style.color = "red";
        FailedReq();
    }
});


function SuccessfulReq() {
    submitButton.disabled = true;
    outputField.innerHTML += " - Redirecting back to home...";
    
    const params = new URLSearchParams(window.location.search);
    const markerIdToDelete = params.get("markerId");
    
    if (markerIdToDelete) {
        fetch(`/TempMarker/DeleteMarker?markerId=${markerIdToDelete}`, { method: 'POST' });
    }

    setTimeout(() => {
        window.location.href = '/Pilot';
    }, 1500);
}
function FailedReq() {

}

function ValidateForm(formData) {
    const fieldsToVerify = ['Type', 'Description', 'Lat', 'Lng', 'HeightM', 'Organization', 'ObstacleCategory', 'Source'];

    for (const [name, value] of formData.entries()) {
        if (value === '' && fieldsToVerify.includes(name)) {
            return false;
        }
    }
    return true;
}
