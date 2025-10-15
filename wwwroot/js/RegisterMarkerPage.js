
const form = document.querySelector("form");
const outputField = document.getElementById('returnMsg');
const submitButton = document.querySelector('button[type="submit"]');


form.addEventListener("submit", async (event) => {
    event.preventDefault(); // Stop the default form submission
    const formData = new FormData(form);

    if (!ValidateForm(formData)) {
        outputField.innerHTML += " You forgot to fill out some fields!";

    } else {
        const response = await fetch(form.action, {
            method: form.method.toUpperCase(), // "POST"
            body: formData
        }).then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.json();
        })
            .then(reply => {
                outputField.innerHTML = reply.message;
                if (reply.success) {
                    SuccessfulReq();
                } else {
                    FailedReq();
                }

            })
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
