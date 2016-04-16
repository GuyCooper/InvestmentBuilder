/// <reference path="jquery-1.10.2.js" />
/// <reference path="jquery-ui.js" />
$(document).ready(function () {
    $('.datetime').datepicker({
        dateFormat: "dd/mm/yy",
        showOn: "button",
        gotoCurrent: true,
        showAnim: 'fold',
        buttonImage: "/Content/calendar.png",
        buttonImageOnly: true
    });
});
