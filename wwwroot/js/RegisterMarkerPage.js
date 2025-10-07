const form = document.querySelector("form");
const outputField = document.getElementById('returnMsg');
const submitButton = document.querySelector('button[type="submit"]');


form.addEventListener("submit", async (event) => {
    event.preventDefault(); // Stop the default form submission
    const formData = new FormData(form);

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
});

function SuccessfulReq(msg) {
    submitButton.disabled = true;
    outputField.innerHTML += " - Redirecting to back to home . . .";

    setTimeout(() => {
        location.href = '/';
    }, 1500)
}
function FailedReq() {

}
