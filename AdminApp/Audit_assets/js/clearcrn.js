var canvas, ctx;
var scaleFactor = 1;
var type = "";
//// Session Idle Page refersh
//var IDLE_TIMEOUT = 120; //seconds
//var _idleSecondsTimer = null;
//var _idleSecondsCounter = 0;

//document.onclick = function () {
//    _idleSecondsCounter = 0;
//};

//document.onmousemove = function () {
//    _idleSecondsCounter = 0;
//};

//document.onkeypress = function () {
//    _idleSecondsCounter = 0;
//};

//_idleSecondsTimer = window.setInterval(CheckIdleTime, 1000);

//function CheckIdleTime() {
//    _idleSecondsCounter++;
//    var oPanel = document.getElementById("SecondsUntilExpire");
//    if (oPanel)
//        oPanel.innerHTML = (IDLE_TIMEOUT - _idleSecondsCounter) + "";
//    if (_idleSecondsCounter >= IDLE_TIMEOUT) {
//        window.clearInterval(_idleSecondsTimer);
//        //alert("Time expired!");
//        document.location.href = "/Home/Registration/";
//    }
//}
$(document).ready(function () {
    type = (typeof window.orientation !== "undefined") || (navigator.userAgent.indexOf('IEMobile') !== -1);
});


$("#capture_img_btn_clearcrn").click(function () {

    $("#clearcrn_msg").html("");

    var crn = $('#crn_number').val();
    var device = "";
    if (!type) {
        device = "Web - " + $('#os_details').val() + " - " + $('#browser_details').val();
    } else {
        device = "Mobile";
    }
    if (crn != "") {
        $("#capture_img_btn_clearcrn").attr("disabled", true);
        var get_data = $(this).data('loading-text');
        $("#capture_img_btn_clearcrn").html(get_data);
        $('#capture_img_btn_clearcrn').button('loading');
        $('#crn_number').attr('readonly', true);


        var JsonImage = JSON.stringify({
            crn_no: crn
        });


        var Url = "/Home/Clear_CRN/";
        $.ajax({
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: JsonImage,
            url: Url,
            success: function (response) {
                try {
                    console.log(response);
                    var get_json = JSON.parse(response.Result);
                    console.log(get_json);
                    if (get_json.ClearCrnResult) {
                        if (get_json.ClearCrnResult == "Deleted") {
                            $("#capture_img_btn_clearcrn").html("Clear");
                            $("#capture_img_btn_clearcrn").attr("disabled", false);
                            $("#clearcrn_msg").css("color", "black");
                            $("#clearcrn_msg").html(get_json.ClearCrnResult);
                            $('#crn_number').attr('readonly', false);
                        }
                        else {
                            $("#capture_img_btn_clearcrn").html("Clear");
                            $("#capture_img_btn_clearcrn").attr("disabled", false);
                            $("#clearcrn_msg").css("color", "red");
                            $("#clearcrn_msg").html(get_json.ClearCrnResult);
                            $('#crn_number').attr('readonly', false);
                        }
                    }
                    if (get_json.Message) {
                        $("#capture_img_btn_clearcrn").html("Clear");
                        $("#capture_img_btn_clearcrn").attr("disabled", false);
                        $("#clearcrn_msg").css("color", "red");
                        $("#clearcrn_msg").html("Something went wrong, please try again!");
                        $('#crn_number').attr('readonly', false);
                    }
                }
                catch (err) {
                    $("#capture_img_btn_clearcrn").html("Clear");
                    $("#capture_img_btn_clearcrn").attr("disabled", false);
                    $("#clearcrn_msg").css("color", "red");
                    $("#clearcrn_msg").html(err);
                    $('#crn_number').attr('readonly', false);
                }
            },
            error: function (response) {
                $("#capture_img_btn_clearcrn").html("Clear");
                $("#capture_img_btn_clearcrn").attr("disabled", false);
                $("#clearcrn_msg").css("color", "red");
                $("#clearcrn_msg").html("Something went wrong, please try again!");
                $('#crn_number').attr('readonly', false);
            }
        });
    } else {
        $("#clearcrn_msg").html("Please enter your CRN");
        $("#clearcrn_msg").css("color", "red");
    }
});
function success_popup(message) {
    var html = '<div class="modal fade" id="alert_popup" role="dialog"><div class="modal-dialog"><div class="modal-content">        <div class="modal-body">          <center><h4>"' + message + '"</h4><br><a href="/Home/Registration/" type="button" class="btn btn-default" id="okay_btn">Okay</a>           </center>        </div>      </div>    </div>  </div>';
    document.getElementById("alert_popup_div").innerHTML = html;
    $('#alert_popup').modal('show');
}