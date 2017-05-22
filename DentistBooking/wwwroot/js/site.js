$(document).ready(function () {
    $("[data-toggle='tooltip']").css("cursor", "help").tooltipster();
    var date = new Date();
    var dateString = date.getFullYear() + "-"
        + _.padStart(date.getMonth(), 2, '0')
        + "-"
        + _.padStart(date.getDate(), 2, '0')
        + "T"
        + _.padStart(date.getHours(), 2, '0')
        + ":"
        + _.padStart(date.getMinutes(), 2, '0');
    $("#ReservationTime").attr("min", dateString).val(dateString);
});