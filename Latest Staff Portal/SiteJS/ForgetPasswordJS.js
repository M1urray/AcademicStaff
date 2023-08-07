$(document).ready(function () {
    $("#btnReset").click(function () {
        var username = $("#username").val();
        if (username == "") {
            Swal.fire('Warning', 'Enter Username', 'warning');
            return;
        }
        else {
            var Authedication = {};
            Authedication.Username = username;
            ShowProgress();
            debugger
            $.ajax({
                url: "/Login/ForgotPassword",
                datatype: "json",
                type: "POST",
                data: JSON.stringify({ userlogin: Authedication }),
                contentType: "application/json; charset = utf-8",
                success: function (data) {
                    if (data.success == true) {
                        Swal.fire('Success', data.message, 'success');
                        HideProgress();
                    }
                    else {
                        Swal.fire('Warning', data.message, 'warning');
                        HideProgress();
                    }
                },
                error: function (err) {
                    HideProgress();
                    Swal.fire('Error', err, 'error');
                }
            });
        }
    });
});
$("#username").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnReset").click();
    }
});