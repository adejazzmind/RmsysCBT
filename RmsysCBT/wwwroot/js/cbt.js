window.onbeforeunload = () => "Your test will be submitted automatically.";

document.addEventListener("visibilitychange", () => {
    if (document.hidden) alert("Tab switching is not allowed.");
});

document.addEventListener("contextmenu", e => e.preventDefault());
