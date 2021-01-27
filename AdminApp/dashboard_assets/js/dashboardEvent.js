$(document).ready(function () {
    //Initialization    
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    today = yyyy + '-' + mm + '-' + dd;
    
    var hidden_from_date_value = $('#from_date_value').val();
    var hidden_to_date_value = $('#to_date_value').val();
    
    if (hidden_from_date_value != "") {
        $('#from_date_filter').val(hidden_from_date_value);
    }
    //else {
    //    $('#from_date_filter').val(today);
    //}
    if (hidden_to_date_value != "") {
        $('#to_date_filter').val(hidden_to_date_value);
    }
    else {
        $('#to_date_filter').val(today);
    }

    //Reset Button Function
    $('#reset_btn').on('click', function () {
        window.location.href = '/Home/Dashboard/';
    });


});

function success_popup(message) {
    var html = '<div class="modal fade" id="alert_popup" role="dialog"><div class="modal-dialog"><div class="modal-content">        <div class="modal-body">          <center><h4>"Something went wrong Please try again"</h4><br><a href="/Home/index/" type="button" class="btn btn-default" style="background:#013567;">Okay</a>           </center>        </div>      </div>    </div>  </div>';
    document.getElementById("alert_popup_div").innerHTML = html;
    $('#alert_popup').modal('show');
}




function doCall(link) {
    location.href = link + "&f=" + $('#from_date_filter').val() + "&t=" + $('#to_date_filter').val();
}

function review(Message) {
    var html = '<div class="modal fade" id="alert_popup" role="dialog" data-keyboard="false" data-backdrop="static"><div class="modal-dialog"><div class="modal-content"><div class="modal-body"><h4 class="text-center">' + Message + '</h4><br><center><button id="modalButton1" type="button" class="btn btn-primary">Okay</button></center></div></div>';
    document.getElementById("alert_popup_div").innerHTML = html;
    $('#alert_popup').modal('show');
    $("#modalButton1").click(function () {
        $('#alert_popup').modal('hide');
    });
}

$('#Dashboard').submit(function (event) {
    event.preventDefault();
    var JsonData = JSON.stringify({ f: $("#from_date_filter").val(), t: $("#to_date_filter").val() });
    var get_data = $('#submit_btn').attr('data-loading-text');
    $('#submit_btn').html(get_data);
    $('#submit_btn').button('loading');
    $('#submit_btn').attr("disabled", true);
    $('#reset_btn').attr("disabled", true);

    //console.log(JsonData);
    $.ajax({
        type: "POST",
        contentType: "application/json",
        datatype: "json",
        data: JsonData,
        url: "/Home/PostDashboardSuccess",
        success: function (response) {
            try {
                if (response.StatusCode == 200) {
                    $('#LatestUpdate').text(response.LatestUpdate);
                    $('#TotalReg').html(response.TotalReg);
                    $('#MobileRegAndroid').html(response.MobileRegAndroid);
                    $('#MobileRegIOS').html(response.MobileRegIOS);
                    $('#NetbankingReg').html(response.NetbankingReg);
                    $('#BranchReg').html(response.BranchReg);
                    $('#TotalVer').html(response.TotalVer);
                    $('#MobileVerAndroid').html(response.MobileVerAndroid);
                    $('#MobileVerIOS').html(response.MobileVerIOS);
                    $('#NetbankingVer').html(response.NetbankingVer);
                    $('#BranchVer').html(response.BranchVer);
                    $('#RegDropOffs').html(response.RegDropOffs);
                    $('#VeriDropOffs').html(response.VeriDropOffs);
                    $('#Gate1Failure').html(response.Gate1Failure);
                    $('#Gate2Failure').html(response.Gate2Failure);
                    $('#Gate3Failure').html(response.Gate3Failure);

                    $('#submit_btn').html("Submit");
                    $('#submit_btn').attr("disabled", false);
                    $('#reset_btn').attr("disabled", false);
                }
                else {
                    review("Something Went Wrong, Please Try Again");
                    $('#submit_btn').html("Submit");
                    $('#submit_btn').attr("disabled", false);
                    $('#reset_btn').attr("disabled", false);
                }
            }
            catch (err) {
                review("Something Went Wrong, Please Try Again");
                $('#submit_btn').html("Submit");
                $('#submit_btn').attr("disabled", false);
                $('#reset_btn').attr("disabled", false);
            }
        },
        error: function (response) {
            review("Something Went Wrong, Please Try Again");
            $('#submit_btn').html("Submit");
            $('#submit_btn').attr("disabled", false);
            $('#reset_btn').attr("disabled", false);
        }
    });
});