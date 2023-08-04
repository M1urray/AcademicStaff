$(document).ready(function () {
    $("#btnLogin").click(function () {
        var username = $("#username").val();
        var password = $("#userpassword").val();
        if (username == "") {
            Swal.fire('Warning', 'Enter Username', 'warning');
            return;
        }
        else if (password == "") {
            Swal.fire('Warning', 'Enter Your Password', 'warning');
            return;
        }
        else {
            var UserCred = {};
            UserCred.Username = username;
            UserCred.Password = password;
            ShowProgress();
            $.ajax({
                url: "/Login/LoginUser",
                datatype: "json",
                type: "POST",
                data: JSON.stringify({ userlogin: UserCred }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        window.location = data.message;
                    }
                    else {
                        HideProgress();
                        Swal.fire('Warning', data.message, 'warning');
                    }
                },
                error: function () {
                    if (data.success == false) {
                        HideProgress();
                        Swal.fire('Error', data.message, 'error');
                    }
                }
            });
        }
    });
});
$("#username").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnLogin").click();
    }
});
$("#userpassword").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnLogin").click();
    }
});
setInterval("getData()", 100);
function getData() {
    var dt = new Date();
    var time = dt.getHours();
    var Greetings = "";
    if ((time >= 0) && (time < 12)) {
        Greetings = "Good Morning !";
    }
    else if ((time >= 12) && (time < 17)) {
        Greetings = "Good Afternoon !";
    }
    else {
        Greetings = "Good Evening !";
    }
    $("#h5").html(Greetings);
}