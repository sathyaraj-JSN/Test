//is number validation
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

//Filter From Date Onchange Validation
function FDate(this_id) {
    $('#filter_date_err_msg').html("");
    var UserDate = document.getElementById(this_id).value;
    var current_date = new Date();
    var todate_get = $('#to_date_filter').val();
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = yyyy + '-' + mm + '-' + dd;

    if (isNaN(new Date(UserDate).getTime())) {
        $('#filter_date_err_msg').html("From Date should not be empty");
        $('#' + this_id).val(today);
        $('#to_date_filter').val(today);
    }
    if (new Date(UserDate).getTime() == current_date.getTime()) {
        $('#filter_date_err_msg').html("");
    }
    if (new Date(todate_get).getTime() < new Date(UserDate).getTime()) {
        $('#filter_date_err_msg').html("From Date should be less than To Date");
        $('#' + this_id).val(today);
        $('#to_date_filter').val(today);
        return false;
    }
    return true;
}

//Filter To Date Onchange Validation
function TDate(this_id) {
    $('#filter_date_err_msg').html("");
    var UserDate = document.getElementById(this_id).value;
    var current_date = new Date();
    var fromdate_get = $('#from_date_filter').val();
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = yyyy + '-' + mm + '-' + dd;
    if (isNaN(new Date(UserDate).getTime())) {
        $('#filter_date_err_msg').html("To Date should not be empty");
        $('#' + this_id).val(today);
        $('#to_date_filter').val(today);
    }
    if (new Date(UserDate).getTime() == current_date.getTime()) {
        $('#filter_date_err_msg').html("");
    }


    if (new Date(fromdate_get).getTime() > new Date(UserDate).getTime()) {
        $('#filter_date_err_msg').html("To Date should be greater than From Date");
        $('#' + this_id).val(today);
        $('#from_date_filter').val(today);
        return false;
    }

    return true;
}