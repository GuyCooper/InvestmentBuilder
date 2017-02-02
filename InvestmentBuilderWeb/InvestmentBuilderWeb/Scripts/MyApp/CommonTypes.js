/// <reference path="CommonTypes.js" />

$(document).ready(function () {
    $("#addReceiptButton").click(onAddReceipt);
    $("#addPaymentButton").click(onAddPayment);
    $('.editTradeButton').click(onEditTrade);
    $('.datetime').click(onDisplayDateTimePicker);

    $('#errorAlert').bind('closed.bs.alert', onCloseErrors);

});

function asyncGetParameters() {
    var parameter = $("#ParameterType").val();
    $.ajax({
        type: "GET",
        url: 'GetParametersForTransaction',
        data: { ParameterType: parameter},//$.param('ParameterType', 'Subscription'), //$("#ParameterType").val()),
        dataType: "json",
        success: function (result, status, sender) {
            var content = "<div id=\"parameters\"><select name=\"Parameter\" class=\"form-control\">";
            $.each(result, function (key, value) {
                content += "<option value=\"" + value + "\">" + value + "</option>"
            });
            content += "</select></div>";
            $("#parameters").replaceWith(content);
        },
        error: function OnGetParametersError(sender, status, error) {
            alert("error:" + error.toString())
        }
    });
}

function onAddDialogContents(transactionurl, parameters) {
    if (document.location.pathname == "/") {
        transactionurl = "InvestmentRecord/" + transactionurl;
    }

    $.ajax({
        url: transactionurl,
        data: parameters,
        dataType: "html",
        type: "GET",
        success: function (result) {
            $('#dialogContents').empty().append(result);
        }
    });
}

function onAddReceipt() {
    onAddDialogContents('AddReceiptTransaction', null);
}

function onAddPayment() {
    onAddDialogContents('AddPaymentTransaction', null);
}
   
function onEditTrade() {
    //var parameter = $("#tradeNameValue").val().toString();
    //alert("on edit trade!. val = " + parameter);
    var parameter = $(this).attr('Value');
    onAddDialogContents('EditTrade', { name: parameter });
}
            
function onCloseErrors() {
    $.ajax({
        url: 'ClearErrors',
        dataType: "text",
        type: "GET"
    });
}

function onDisplayDateTimePicker() {
    $('.datetime').datepicker({
        dateFormat: "dd/mm/yy",
    });
}
