
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