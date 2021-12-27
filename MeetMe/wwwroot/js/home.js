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

//function onGetData(res)
//{
//    console.log(res);
//    alert(res);
//}

function getIp()
{
    $.ajax({
        url: "https://localhost:44346/Home/GetUsers",
        type: 'GET',
        dataType: 'json', // tutaj mozna podac jakiego typu sa dane wynikowe
        success: function (res) { //funkcja anonimowa cos jak w C#: res => { console.log(res); alert(res); }
            console.log(res);
            alert(res.ip);
        }
    });
}

function getUsers() {
    $.ajax({
        url: "/Home/GetUsers",
        type: 'GET',
        success: function (partialView) { //funkcja anonimowa cos jak w C#: res => { console.log(res); alert(res); }
            //document.getElementById("users").append(partialView)
            $('#users').append(partialView);
        }
    });
}