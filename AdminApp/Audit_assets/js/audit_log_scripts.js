$(document).ready(function () {
    //Initialization
    var set_value = 0;
    var send_ajax = 0;
    $('.even').hide();
    var compare_date1 = "";
    var compare_date2 = "";
    var current_date = new Date();
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    today = yyyy + '-' + mm + '-' + dd;

    var hidden_crn_value = $('#crn_value').val();
    var hidden_event_value = $('#event_value').val();
    var hidden_status_value = $('#status_value').val();
    var hidden_gate_value = $('#gate_value').val();
    var hidden_from_date_value = $('#from_date_value').val();
    var hidden_from_date_value = $('#from_date_value').val();
    var hidden_to_date_value = $('#to_date_value').val();
    var hidden_version = $('#version_value').val();


    if (hidden_from_date_value == "") {
        compare_date1 = today;
    } else {
        compare_date1 = hidden_from_date_value;
    }
    if (hidden_to_date_value == "") {
        compare_date2 = today;
    } else {
        compare_date2 = hidden_from_date_value;
    }
    if (hidden_crn_value != "" || hidden_event_value != "" || hidden_status_value != "" || hidden_gate_value != "" || compare_date1 != today || compare_date2 != today || hidden_version!="") {
        if ($('#collapseExample').hasClass("show")) {
            $('#collapseExample').removeClass("show");
        }
        else {
            $('#collapseExample').addClass("show");
        }
    }

    //if (hidden_crn_value == "" || hidden_event_value == "" || hidden_status_value == "" || hidden_gate_value == "" || hidden_from_date_value == "" || hidden_to_date_value == "") {
    //    $('#collapseExample').removeClass("show");
    //}


    
    if (hidden_crn_value != "") {
        $('#cf').val(hidden_crn_value);
    }
    if (hidden_event_value != "") {
        $('#events_filter').val(hidden_event_value);
    }
    if (hidden_status_value != "") {
        $('#status_filter').val(hidden_status_value);
    }
    if (hidden_gate_value != "") {
        $('#gate_filter').val(hidden_gate_value);
    }
    if (hidden_from_date_value != "") {
        $('#from_date_filter').val(hidden_from_date_value);
    }
    else {
        $('#from_date_filter').val(today);
    }
    if (hidden_to_date_value != "") {
        $('#to_date_filter').val(hidden_to_date_value);
    }
    else {
        $('#to_date_filter').val(today);
    }
    if (hidden_version != "") {
        $('#version_filter').val(hidden_version);
    }

    // Add event listener for opening and closing details
    $('#audit_log_table tbody').on('click', 'td.expand_arrow', function () {
        var audit_id = $(this).attr('id');
        $('table tr#'+ audit_id).focus();
        if (set_value == 0) {
            set_value = audit_id;
            $('#even_sub_' + audit_id).find("tr:gt(0)").remove();
            $('#arrow_' + audit_id + ' i').removeClass('fa fa-angle-right');
            $('#arrow_' + audit_id + ' i').addClass('fa fa-angle-down');
            send_ajax = 1;
        } else if (set_value == audit_id) {
            send_ajax = 0;
            $('#even_' + audit_id).hide();
            $('#even_sub_' + audit_id).find("tr:gt(0)").remove();
            $('#arrow_' + audit_id + ' i').removeClass('fa fa-angle-down');
            $('#arrow_' + audit_id + ' i').addClass('fa fa-angle-right');
            set_value = 0;
        } else if (set_value != 0) {
            $('#even_' + set_value).hide();
            $('#even_sub_' + audit_id).find("tr:gt(0)").remove();
            $('#arrow_' + set_value + ' i').removeClass('fa fa-angle-down');
            $('#arrow_' + set_value + ' i').addClass('fa fa-angle-right');
            $('#arrow_' + audit_id + ' i').removeClass('fa fa-angle-right');
            $('#arrow_' + audit_id + ' i').addClass('fa fa-angle-down');
            set_value = audit_id;
            send_ajax = 1;
        }
        if (send_ajax == 1) {
            var get_expand_audit_id = $(this).attr('expand_audit_id');
            var get_event_name = $(this).attr('get_event_name');
            var send_expand_audit_id = JSON.stringify({
                audit_id: get_expand_audit_id,
                event_name: get_event_name
            });
            $('table tr#' + audit_id).focus();
            $.ajax({
                type: "POST",
                contentType: "application/json",
                datatype: "json",
                data: send_expand_audit_id,
                url: "/Home/AuditLogExpandFetch/",
                success: function (data) {
                    try {
                        var gate_img = [];
                        $('#even_'+ audit_id).show();
                        window.location.hash = '#even_' + audit_id;
                        for (i = 0; i < data.length; i++) {
                            var get_json = data[i].JSON_RESPONSE;
                            var table_td_json = get_json.substring(0, 90);
                            $('#even_sub_' + audit_id + ' tr:last').after('<tr><td><div class="row"><div class="col-md-12 col-sm-12 col-lg-12 col-xs-12"><div id="img_get_' + data[i].AUDIT_ID + '" class="align_center"></div></div></div></td><td><p style="min-width:100px;">' + data[i].GATE_NAME + '</p></td><td><a title="click to expand" data-toggle="modal" data-target="#myModal_' + data[i].AUDIT_ID + '" style="cursor:pointer;"><p class="json_td_length"><pre>' + table_td_json + '</pre><span>...</span></p></a></td><td><p>' + data[i].STATUS_NAME + '</p><div id="myModal_' + data[i].AUDIT_ID + '" class="modal fade" role="dialog"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><h4 class="modal-title">Full JSON Structure</h4><button type="button" class="close" data-dismiss="modal">&times;</button></div><div class="modal-body"><p><pre>' + data[i].JSON_RESPONSE + '</pre></p></div></div></div></div></td></tr>');
                            if (data[i].GATE_NAME != "1000")
                            {
                                $("#img_get_" + data[i].AUDIT_ID).html('<img src="' + data[i].Image_get + '" id="gate_img_' + data[i].AUDIT_ID + '" class="user-avatar size-100">');
                                if (data[i].STATUS_NAME == "Success")
                                {
                                    gate_img.push(data[i].Image_get);
                                }
                            }
                            else {
                                if (gate_img.length == "2") {
                                    $("#img_get_" + data[i].AUDIT_ID).html('<div class="manage-margin d-flex align-items-center flex-wrap"><img class="user-avatar size-60" alt="Azure Registration Image 1" src="' + gate_img[0] + '"><img class="user-avatar size-60" alt="Azure Registration Image 2" src="' + gate_img[1] + '"></div>');
                                }
                                if (gate_img.length == "1") {
                                    $("#img_get_" + data[i].AUDIT_ID).html('<img src="' + gate_img[0] + '" id="gate_img_' + data[i].AUDIT_ID + '" class="user-avatar size-100">');
                                }
                            }
                            
                        }
                        console.log(gate_img);
                    }
                    catch (err) {
                        $('#even_' + audit_id).hide();
                        $('#arrow_' + audit_id + ' i').removeClass('fa fa-angle-right');
                        $('#arrow_' + audit_id + ' i').addClass('fa fa-angle-down');
                        success_popup(err);
                    }
                }
            });
        }
    });


    //Clear Audit Log Click Function
    $('#clear_audit_log').on('click', function () {

        var data = JSON.stringify({
            data: "clear"
        });

        $.ajax({
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: data,
            url: "/Home/TruncateAuditLog/",
            success: function (data) {
                document.location.reload(true);
            }
        });

    });


    //Reset Button Function
    $('#reset_btn').on('click', function () {
        window.location.href = '/Home/Index/';
    });


});

function success_popup(message) {
    var html = '<div class="modal fade" id="alert_popup" role="dialog"><div class="modal-dialog"><div class="modal-content">        <div class="modal-body">          <center><h4>"Something went wrong Please try again"</h4><br><a href="/Home/index/" type="button" class="btn btn-default" style="background:#013567;">Okay</a>           </center>        </div>      </div>    </div>  </div>';
    document.getElementById("alert_popup_div").innerHTML = html;
    $('#alert_popup').modal('show');
}