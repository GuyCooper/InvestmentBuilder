
    $('.datetime').datepicker({
        dateFormat: "dd/mm/yy",
        showOn: "button",
        gotoCurrent: true,
        showAnim: 'fold',
        buttonImage: "/Content/calendar.png",
        buttonImageOnly: true
    });

    $(document).ready(function () {

        //$.ajax({
        //    url: '/InvestmentRecord/GetAccounts',
        //    type: 'GET',
        //    dataType: 'html',
        //    success: function (result) {
        //        $("#accountsList").append(result)
        //    }
        //});

        $.ajax({
            url: '/Itinerary/GetAdParts',
            dataType: "json",
            type: "POST",
            success: function (result) {
                var content = "<ul>";
                $.each(result, function (key, value) {
                    content += "<li ><a href = '"
                    + value.url + "' title='"
                    + value.caption + "'>"
                    + value.image + "</a></li>";
                });
                content += "</ul>";
                $("#ads").append(content);
            }
        });


        $("#helpTrigger").click(getHelp);
    });

        function getHelp() {
            $.ajax({
                url: '/Itinerary/GetHelp',
                dataType: "json",
                type: "POST",
                success: function (result) {
                    $("#helpContent").empty().append(result);
                    $("#helpContent").show();
                    $("#helpTrigger").removeClass("helpTrigger")
                    .addClass("closeTrigger");
                    $("#helpTrigger").unbind();
                    $("#helpTrigger").click(closeHelp);
                }
            });
        }

    function closeHelp() {
        $("#helpContent").empty();
        $("#helpContent").hide();
        $("#helpTrigger").removeClass("closeTrigger")
        .addClass("helpTrigger");
        $("#helpTrigger").unbind();
        $("#helpTrigger").click(getHelp);
    }

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


    
            