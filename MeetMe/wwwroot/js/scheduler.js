
function getScheduler(year, month) {
    $.ajax({
        url: "/Scheduler/GetScheduler?year=" + year + "&month=" + month,
        type: 'GET',
        success: function (partialView) { //funkcja anonimowa cos jak w C#: res => { console.log(res); alert(res); }
            console.log(partialView);
            $('#scheduler').html(partialView);
        }
    });
   
}

function getMeeting(id) {
    $.ajax({
        url: "/Meetings/GetMeeting?id=" + id,
        type: 'GET',
        success: function (partialView) { //funkcja anonimowa cos jak w C#: res => { console.log(res); alert(res); }
            console.log(partialView);
            $('#meetingModalBody').html(partialView);
        }
    });
}

function getMeetings(year, month, day) {
    $.ajax({
        url: "/Meetings/GetDayMeetings?year=" + year + "&month=" + month + "&day=" + day,
        type: 'GET',
        success: function (partialView) { //funkcja anonimowa cos jak w C#: res => { console.log(res); alert(res); }
            console.log(partialView);
            $('#meetingModalBody').html(partialView);
        }
    });
}