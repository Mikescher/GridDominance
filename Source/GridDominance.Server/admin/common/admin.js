
function ShowExpandedColumn(id, text) {
    if ($("#tr_prev_"+id).css('visibility') !== 'visible') {
        $(".tab_prev").css("visibility", "collapse");
        $(".tab_prev").css("display", "none");
        $("#td_prev_"+id).html(text);
        $("#tr_prev_"+id).css("visibility", "visible");
        $("#tr_prev_"+id).css("display", "table-row");
    } else {
        $(".tab_prev").css("visibility", "collapse");
        $(".tab_prev").css("display", "none");
    }
}

function HideExpandedColumn(id, text) {
    $(".tab_prev").css("visibility", "collapse");
    $(".tab_prev").css("display", "none");
}

function ShowRemoteExpandedColumn(id, ident) {
    if ($("#tr_prev_"+id).css('visibility') !== 'visible') {
        $(".tab_prev").css("visibility", "collapse");
        $(".tab_prev").css("display", "none");

        $.get('logquery.php?id=' + ident, function( data ) {
            $("#td_prev_"+id).html(data);
            $("#tr_prev_"+id).css("visibility", "visible");
            $("#tr_prev_"+id).css("display", "table-row");
        });

    } else {
        $(".tab_prev").css("visibility", "collapse");
        $(".tab_prev").css("display", "none");
    }
}

$(function() {


    $("[data-tabheader]").each(function(idx) {

        let tabcontainerid = $(this).data('tabcontainerid');
        let active = $(this).hasClass('samtabactive');
        let tabid = $(this).data('tabid');


        $("[data-tabcontent]").each(function(idx) {
            if ($(this).data('tabcontainerid') === tabcontainerid && $(this).data('tabid') === tabid) {

                if (active) {
                    $(this).css("display", "");
                    $(this).css("visibibility", "");
                } else {
                    $(this).css("display", "none");
                    $(this).css("visibibility", "hidden");
                }

            }
        });

    });


    $("[data-tabheader]").each(function(idx) {
        $(this).click(function(idx){

            let tabcontainerid = $(this).data('tabcontainerid');
            let tabid = $(this).data('tabid');
            let active = $(this).hasClass('samtabactive');

            $("[data-tabcontent]").each(function(idx) {
                if ($(this).data('tabcontainerid') === tabcontainerid) {

                    if ($(this).data('tabid') === tabid && !active) {
                        $(this).css("display", "");
                        $(this).css("visibibility", "");
                    } else {
                        $(this).css("display", "none");
                        $(this).css("visibibility", "hidden");
                    }

                }
            });


            $("[data-tabheader]").each(function(idx) {
                if ($(this).data('tabcontainerid') === tabcontainerid) {

                    if ($(this).data('tabid') === tabid && !active) {
                        $(this).addClass('samtabactive');
                    } else {
                        $(this).removeClass('samtabactive');
                    }

                }
            });

        });
    });

});