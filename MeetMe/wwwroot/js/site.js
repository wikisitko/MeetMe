let selectedAttendenceId = -1;

////function deleteRequest(url) {
////    const response = await fetch(url, {
////        method: 'DELETE',
////        headers: {
////            'Content-type': 'application/json'
////        }
////    });
////    console.log(response.text);
////    return response.status;
////}

function setModalText(text, attendeeId) {
    document.getElementById("modal-body").innerHTML = text;
    selectedAttendenceId = attendeeId;
}

function deleteAttendance() {
    if (selectedAttendenceId != -1) {
        deleteRequest(selectedAttendenceId);
    }
}

function removeRow(rowId) {
    document.getElementById(rowId + "").remove();
}

/*
 Możliwe są następujące wartości readyState:

0 (niezainicjowane)
1 (w trakcie pobierania)
2 (pobrano)
3 (interaktywne)
4 (gotowe)
*/

function deleteRequest(attendeeId) {
    xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () { //ta funkcja uruchomi sie w momencie otrzymania danych
        if (xhttp.readyState == 4) {
            // wszystko jest OK, odpowiedź została odebrana
            //alert("OK");
            if (xhttp.status == 200) {
                // świetnie!!
                //alert("status 200");
                removeRow(selectedAttendenceId);
            } else {
                // wystąpił jakiś problem z zapytaniem,
                // na przykład odpowiedzią mogło być 404 (Nie odnaleziono)
                // lub 500 (Wewnętrzny błąd serwera)
            }
        } else {
            // ciągle nie gotowe
        }
    };
    xhttp.open('DELETE', '/api/Attendance/Delete/' + attendeeId, true); //true - asynchroniczne
    xhttp.send(null);
}


function addRole() {
    let input = document.getElementById("roleName");
    let roleName = input.value; //jak masz input/select to zeby pobrac wartosc uzywac slowa value, dla innych typow (np div span itp) mozna uzyc innerHTML
    if (roleName == null || roleName == "") {
        alert("Nie wprowadzono nazwy!");
        return;
    }
    input.value = "";
    addRoleApi(roleName);
 }

function addRoleToTable(roleName, roleId) {
    let table = document.getElementById("roleTable");
    let row = table.insertRow(); //tworzy nowy wiersz i dodaje go na koniec
    let cell = row.insertCell(); //dodaje kolumne do wiersza
    cell.innerHTML = roleName;
    cell = row.insertCell();
    cell.innerHTML = '<a class="btn btn-danger" href="/Role/Delete/'+ roleId +'"><svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="white" class="bi bi-trash-fill" viewBox="0 0 16 16"><path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z" /></svg></a>'
}


function addRoleApi(roleName) {
    xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () { //ta funkcja uruchomi sie w momencie otrzymania danych
        if (xhttp.readyState == 4) {
            // wszystko jest OK, odpowiedź została odebrana
            //alert("OK");
            if (xhttp.status == 200) {
                // świetnie!!
                //alert("status 200");
                let roleId = xhttp.responseText;
                addRoleToTable(roleName, roleId);
            } else {
                // wystąpił jakiś problem z zapytaniem,
                // na przykład odpowiedzią mogło być 404 (Nie odnaleziono)
                // lub 500 (Wewnętrzny błąd serwera)
            }
        } else {
            // ciągle nie gotowe
        }
    };
    
    xhttp.open('POST', '/api/Roles'); //true - asynchroniczne
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify({ "Name" : roleName }));
}