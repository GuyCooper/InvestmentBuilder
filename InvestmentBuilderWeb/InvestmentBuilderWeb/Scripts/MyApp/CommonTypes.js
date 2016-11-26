/// <reference path="CommonTypes.js" />

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
    alert('edit trade: ' + parameter);
    onAddDialogContents('EditTrade', { name: parameter });
}
            
