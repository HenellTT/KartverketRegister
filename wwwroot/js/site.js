// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//dark mode toggle
    document.addEventListener('DOMContentLoaded', () => {
        const body = document.body;

        document.getElementById('Light_Btn').onclick = () => {
            body.classList.remove('dark-mode');
            sendTheme('light');
        };

        document.getElementById('Dark_Btn').onclick = () => {
            body.classList.add('dark-mode');
            sendTheme('dark');
        };

        function sendTheme(mode) {
            fetch('/Home/SetMode', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: `mode=${mode}`
            });
        }
    });