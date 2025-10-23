// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//dark mode toggle
document.addEventListener('DOMContentLoaded', () => { //venter til html er lastet
    const toggle = document.getElementById('darkModeToggle'); //henter toggle elementet
    const body = document.body; //kobler til body elementet
    toggle.checked = body.classList.contains('dark-mode'); //toggle "på" hvis den er i dark mode
    toggle.addEventListener('change', () => { //når toggle endres, når den klikkes så kjører denne funksjonen
        const mode = toggle.checked ? 'dark' : 'light'; //hvis checked er true, sett mode til dark, ellers light
        body.classList.toggle('dark-mode', toggle.checked); //legger til eller fjerner dark mode på bodyen

        //sender mode til serveren
        fetch('/Home/SetMode', { 
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ mode: mode })
        });