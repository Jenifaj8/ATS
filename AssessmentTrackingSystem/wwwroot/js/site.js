// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    var themeToggleButton = document.querySelector("[data-theme-toggle]");
    var themeLabel = document.querySelector("[data-theme-label]");
    var themeIcon = document.querySelector("[data-theme-icon]");

    if (!themeToggleButton || !themeLabel || !themeIcon) {
        return;
    }

    function applyTheme(theme) {
        var isDarkMode = theme === "dark";

        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("app-theme", theme);

        themeLabel.textContent = isDarkMode ? "Light Mode" : "Dark Mode";
        themeIcon.className = isDarkMode ? "bi bi-sun-fill" : "bi bi-moon-stars-fill";
    }

    var currentTheme = document.documentElement.getAttribute("data-theme") || "light";
    applyTheme(currentTheme);

    themeToggleButton.addEventListener("click", function () {
        var nextTheme = document.documentElement.getAttribute("data-theme") === "dark" ? "light" : "dark";
        applyTheme(nextTheme);
    });
});
