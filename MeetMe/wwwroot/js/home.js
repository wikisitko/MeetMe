let welcomeOpened = false;

function showWelcome() {
    let element = document.getElementById("welcome");
    element.style.display = welcomeOpened ? "none" : ""; //teraz element pojawi/schowa sie na ekranie
    welcomeOpened = !welcomeOpened;
}

function sum() {
    let inputs = document.getElementsByName("input"); //w rezultacie dostaniemy liste elementow html
    let result = 0;
    inputs.forEach(x => result += parseInt(x.value));
    document.getElementById("result").innerHTML = result + "";

    let span = document.createElement("span");
    span.innerHTML = result + "";
    document.getElementById("calculator").appendChild(span);
}