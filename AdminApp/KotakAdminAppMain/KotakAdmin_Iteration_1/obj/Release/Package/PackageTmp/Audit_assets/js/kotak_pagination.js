$(document).ready(function () {
    //Intialization
    var total_records_count = $('#total_records_count').val();
    var before_point_number, final_number_of_pages, first_offset_value;
    var pagination_array = ["Previous"];
    var url_value = window.location.href;
    var url_value_get = new URL(url_value);
    var first_offset_value_get = url_value_get.searchParams.get("v");
    $('#filter_date_err_msg').html("");
    $('#total_records_showing').html(total_records_count + ' ');


    //Get Number of pages
    var number_of_pages = (total_records_count / 100).toString();
    var number_of_pages_compare = (total_records_count / 100);

    //Creating array Elements for li in pagination
    if (Number(number_of_pages_compare) === number_of_pages_compare && number_of_pages_compare % 1 !== 0) {
        before_point_number = number_of_pages.split(".");
        final_number_of_pages = Number(before_point_number[0]) + 1;
    } else {
        final_number_of_pages = number_of_pages_compare;
    }

    for (i = 0; i < final_number_of_pages; i++) {
        pagination_array.push(i + 1);
    }
    pagination_array.push("Next");

    var ul_list = $('ul.ul_list')
    if (pagination_array.length > 2) {
        $.each(pagination_array, function (i) {
            var li = $('<li/>')
                .addClass('page-item')
                .attr('role', 'menuitem')
                .appendTo(ul_list);
            var aaa = $('<a/>')
                .addClass('page-link pagination_li')
                .attr('id', 'page_number_' + pagination_array[i])
                .attr('page_number', pagination_array[i])
                .text(pagination_array[i])
                .appendTo(li);
        });
    }


    //Less than 3 in pagination array-Not visible 
    if (pagination_array.length <= 3) {
        $('#page_number_Previous').hide();
        $('#page_number_1').hide();
        $('#page_number_Next').hide();
    }

    //Pagination Click function
    $('.pagination_li').on('click', function () {
        var page_number = $(this).attr('page_number');
        if (page_number != 1 && page_number != "Previous" && page_number != "Next") {
            first_offset_value = 0;
            first_offset_value = (page_number * 100) - 100;
        } else if (page_number == "Previous") {
            if (first_offset_value_get != 0) {
                first_offset_value = first_offset_value_get - 100;
            } else {
                first_offset_value = 0;
            }
        } else if (page_number == "Next") {
            var total_records_count_mod = total_records_count % 100;
            var total_records_count_compare = total_records_count - total_records_count_mod;
            if (total_records_count_compare == first_offset_value_get) {
                first_offset_value = first_offset_value_get;
            } else {
                first_offset_value = Number(first_offset_value_get) + 100;
            }
        } else {
            first_offset_value = 0;
        }

        $('#first_offset_value').val(first_offset_value);
        var crn_value = $('#crn_value').val();
        var event_value = $('#event_value').val();
        var status_value = $('#status_value').val();
        var gate_value = $('#gate_value').val();
        var from_date_value = $('#from_date_value').val();
        var to_date_value = $('#to_date_value').val();
        var get_pass = [];
        if (first_offset_value != "" || first_offset_value == 0) {
            if (get_pass.length == 0) {
                get_pass.push("v=" + first_offset_value);
            } else {
                get_pass.push("&v=" + first_offset_value);
            }

        }
        if (crn_value != "") {
            if (get_pass.length == 0) {
                get_pass.push("cf=" + crn_value);
            } else {
                get_pass.push("&cf=" + crn_value);
            }
        }
        if (event_value != "") {
            if (get_pass.length == 0) {
                get_pass.push("ef=" + event_value);
            } else {
                get_pass.push("&ef=" + event_value);
            }
        }
        if (status_value != "") {
            if (get_pass.length == 0) {
                get_pass.push("sf=" + status_value);
            } else {
                get_pass.push("&sf=" + status_value);
            }
        }
        if (gate_value != 0) {
            if (get_pass.length == 0) {
                get_pass.push("gf=" + gate_value);
            } else {
                get_pass.push("&gf=" + gate_value);
            }
        }
        if (from_date_value != "") {
            if (get_pass.length == 0) {
                get_pass.push("f=" + from_date_value);
            } else {
                get_pass.push("&f=" + from_date_value);
            }
        }
        if (to_date_value != "") {
            if (get_pass.length == 0) {
                get_pass.push("t=" + to_date_value);
            } else {
                get_pass.push("&t=" + to_date_value);
            }
        }

        if (get_pass != "") {
            var get_pass_final = get_pass.join("")
            //alert(get_pass_final);
            window.location.href = "/Home/Index?" + get_pass_final;
        } else {
            window.location.href = "/Home/Index?v=" + first_offset_value;
        }

    });
});