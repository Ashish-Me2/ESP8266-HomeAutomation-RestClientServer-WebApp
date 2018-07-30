jQuery.support.cors = true;
var targetURI = "http://homeautomationapi.azurewebsites.net";
//var targetURI = "http://localhost/homeautomation";

$(document).ready(GetDeviceStates());
window.onerror = ErrorHandler;
function ErrorHandler(obj) {
    alert(obj);
}
setInterval(GetDeviceStates, 5000);

function GetDeviceStates() {
    $.ajax(
        {
            type: "GET",
            url: targetURI + '/api/home/GetAllDeviceStatus',
            data: "",
            contentType: "application/json;",
            dataType: "json",
            success: function (data) {
                ApplyDeviceStates(data);
            },
            error: function (xhr, err) {
                //alert("HTTP Status: " + xhr.status);
                //alert("responseText: " + xhr.responseText);
            }
        });
}

function ApplyDeviceStates(roomDeviceData) {
    var roomArr = roomDeviceData.split('@');
    for (i = 0; i < roomArr.length; i++) {
        var devArr = roomArr[i].split('^');
        var RoomName = devArr[0];
        var rowNum = GetTableRowForRoom(RoomName);

        var DevArrData = devArr[1].split(',');
        for (j = 0; j < DevArrData.length; j++) {
            var deviceName = DevArrData[j].split('#')[0];
            var deviceState = DevArrData[j].split('#')[1];
            SetButtonState(rowNum, deviceName, deviceState);
        }
    }
    var d = new Date();
    $(".dtm").text(d.getDate() + "/" + (d.getMonth()+1) + "/" + d.getFullYear() + " - " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds());
}


function SetButtonState(rmNo, deviceName, devState) {
    var newState = "";
    if (devState == "D") {
        newState = "disabled";
    }
    else {
        newState = "pressed";
    }
    switch (deviceName) {
        case "Bulb":
            document.getElementById("B" + (rmNo - 1)).className = (devState == 0) ? "" : "button-" + newState;
            break;
        case "Tubelight":
            document.getElementById("T" + (rmNo - 1)).className = (devState == 0) ? "" : "button-" + newState;
            break;
        case "Fan":
            document.getElementById("F" + (rmNo - 1)).className = (devState == 0) ? "" : "button-" + newState;
            break;
    }
}

function GetTableRowForRoom(roomNameSpecific) {
    retVal = 0;
    var rowCount = document.getElementById("rd").rows.length;
    for (k = 2; k < rowCount; k++) {
        if ((document.getElementById("rd").rows[k].cells[0].innerText) == roomNameSpecific) {
            retVal = k;
            break;
        }
    }
    return retVal;
}


function GetRoomNumberFromButton(btnId) {
    var roomNum = document.getElementById("rd").rows[parseInt(btnId.substring(1, btnId.length)) + 1].cells[0].innerText;
    return roomNum;
}


function Click(btn) {
    var ctrl = document.getElementById(btn.id);
    var device = (btn.id.includes("B")) ? "Bulb" : (btn.id.includes("F")) ? "Fan" : "Tubelight";
    var rmNo = GetRoomNumberFromButton(btn.id);
    var toggle = (ctrl.className.includes("pressed")) ? 0 : 1;
    if (btn.className == "button-disabled") {
        alert("No heartbeat detected from the " + rmNo + " controller. Cannot execute this command at the moment.");
    }
    else {
        $.ajax(
            {
                type: "GET",
                url: targetURI + '/api/home/GetUpdatedDeviceState?DeviceName=' + device + '&RoomName=' + rmNo + '&NewState=' + toggle,
                data: "",
                contentType: "application/json;",
                dataType: "json",
                success: function (data) {
                    GetDeviceStates();
                },
                error: function (xhr, err) {
                    //alert("HTTP Status: " + xhr.status);
                    //alert("responseText: " + xhr.responseText);
                }
            });
    }
}