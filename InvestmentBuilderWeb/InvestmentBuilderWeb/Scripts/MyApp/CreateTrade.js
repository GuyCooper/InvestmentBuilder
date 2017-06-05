'use strict'

function CreateTrade($http) {
    var addTrade = this;
    addTrade.today = function () {
        addTrade.dt = new Date();
    };

    addTrade.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    addTrade.openDatePicker = function () {
        addTrade.datepicker.opened = true;
    };

    addTrade.format = 'dd-MMMM-yyyy';
    addTrade.altInputFormats = ['M!/d!/yyyy'];

    var onTradeSubmitted = function (response) {
        if (response.status == 'fail') {
            //trade submission failed. display error messages in error dialog

        }
    };

    addTrade.datepicker = {
        opened: false
    };

    
    addTrade.submitTrade = function () {
        $http({
            url: 'SubmitTrade',
            method: 'POST',
            params: {
                transactionDate: addTrade.dt,
                name: addTrade.Name,
                symbol: addTrade.Symbol,
                quantity: addTrade.Quantity,
                scalingFactor: addTrade.ScalingFactor,
                currency: addTrade.currency,
                exchange: addTrade.Exchange,
                totalCost: addTrade.totalCost
            }
        })
        .then(onTradeSubmitted);
    };

    addTrade.cancel = function () {

    };

    addTrade.today();
}