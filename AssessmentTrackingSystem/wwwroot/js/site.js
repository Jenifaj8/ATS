// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    var themeToggleButtons = document.querySelectorAll("[data-theme-toggle]");

    if (!themeToggleButtons.length) {
        return;
    }

    function applyTheme(theme) {
        var isDarkMode = theme === "dark";

        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("app-theme", theme);

        themeToggleButtons.forEach(function (button) {
            var themeIcon = button.querySelector("[data-theme-icon]");
            var themeLabel = button.querySelector("[data-theme-label]");

            if (themeIcon) {
                themeIcon.className = isDarkMode ? "bi bi-sun-fill" : "bi bi-moon-stars-fill";
            }

            if (themeLabel) {
                themeLabel.textContent = isDarkMode ? "Light Mode" : "Dark Mode";
            }

            button.setAttribute(
                "aria-label",
                isDarkMode ? "Switch to light mode" : "Switch to dark mode"
            );
        });
    }

    var currentTheme = document.documentElement.getAttribute("data-theme") || "light";
    applyTheme(currentTheme);

    themeToggleButtons.forEach(function (button) {
        button.addEventListener("click", function () {
            var nextTheme =
                document.documentElement.getAttribute("data-theme") === "dark"
                    ? "light"
                    : "dark";

            applyTheme(nextTheme);
        });
    });
});